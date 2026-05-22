using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class AesEncryptionService
{
    /// <summary>
    /// SEC-13: Versleutelt een platte tekst beschrijving naar een .enc kamerbestand met een AES-256 sleutel.
    /// Slaat de willekeurig gegenereerde IV op in de eerste 16 bytes van het bestand (Optie A van decryptie).
    /// </summary>
    /// <param name="roomId">Het ID van de kamer (bepaalt de bestandsnaam).</param>
    /// <param name="keyShare">De keyshare die is opgehaald bij de API.</param>
    /// <param name="passphrase">Het door de ontwerper gekozen wachtwoord.</param>
    /// <param name="plainText">De leesbare omschrijving van de kamer.</param>
    /// <returns>Een statusbericht dat aangeeft of de encryptie geslaagd is.</returns>
    public static string EncryptRoomFile(string roomId, string keyShare, string passphrase, string plainText)
    {
        try
        {
            // SEC-15: Veilige IO-operatie. Zorg dat de map bestaat voor we gaan schrijven.
            string directoryPath = "Rooms";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string filePath = Path.Combine(directoryPath, $"{roomId}.enc");

            // SEC-13: Genereer de 256-bit AES-sleutel op basis van SHA256(keyshare + ":" + passphrase)
            byte[] keyBytes;
            using (SHA256 sha256 = SHA256.Create())
            {
                string combined = $"{keyShare}:{passphrase}";
                keyBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
            }

            byte[] iv;
            byte[] cipherText;

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;

                // SEC-13: Genereer een unieke, cryptografisch veilige IV (Initialization Vector)
                aes.GenerateIV();
                iv = aes.IV;

                // Voer de werkelijke AES-encryptie uit
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs, Encoding.UTF8))
                        {
                            sw.Write(plainText);
                        }
                    }
                    cipherText = ms.ToArray();
                }
            }

            // Professionele aanpak (Optie A): Plak de 16 bytes van de IV vòòr de cipherText
            byte[] combinedBytes = new byte[iv.Length + cipherText.Length];
            Array.Copy(iv, 0, combinedBytes, 0, iv.Length);
            Array.Copy(cipherText, 0, combinedBytes, iv.Length, cipherText.Length);

            // Schrijf de gecombineerde bytes veilig weg naar de harde schijf
            File.WriteAllBytes(filePath, combinedBytes);

            return $"[Succes] Kamer '{roomId}' succesvol versleuteld en opgeslagen in {filePath}.";
        }
        catch (Exception ex)
        {
            // SEC-15: Algemene foutafhandeling om crashes tijdens het genereren te voorkomen
            return $"[Fout] Er is een fout opgetreden tijdens het versleutelen: {ex.Message}";
        }
    }

    /// <summary>
    /// SEC-13: Ontsleutelt een .enc kamerbestand met een AES-256 sleutel gegenereerd uit de keyshare en passphrase.
    /// </summary>
    /// <param name="roomId">Het ID van de kamer (bepaalt de bestandsnaam).</param>
    /// <param name="keyShare">De keyshare die is opgehaald bij de API.</param>
    /// <param name="passphrase">Het door de speler ingevoerde wachtwoord.</param>
    /// <returns>De leesbare, ontsleutelde beschrijving van de kamer of een foutmelding.</returns>
    public static string DecryptRoomFile(string roomId, string keyShare, string passphrase)
    {
        try
        {
            // SEC-15: Veilige IO-operatie. Controleer eerst of het bestand bestaat.
            string filePath = $"Rooms/{roomId}.enc";
            if (!File.Exists(filePath))
            {
                return "[Fout] Versleuteld kamerbestand niet gevonden op de client.";
            }

            // SEC-13: Genereer de 256-bit AES-sleutel op basis van SHA256(keyshare + ":" + passphrase)
            byte[] keyBytes;
            using (SHA256 sha256 = SHA256.Create())
            {
                string combined = $"{keyShare}:{passphrase}";
                keyBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
            }

            // Lees de rauwe versleutelde bytes in
            byte[] fileBytes = File.ReadAllBytes(filePath);

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;

                byte[] iv = new byte[16];
                byte[] cipherText;

                if (fileBytes.Length > 16)
                {
                    // We gaan uit van Optie A: IV herleiden uit de eerste 16 bytes van het bestand
                    Array.Copy(fileBytes, 0, iv, 0, 16);
                    cipherText = new byte[fileBytes.Length - 16];
                    Array.Copy(fileBytes, 16, cipherText, 0, cipherText.Length);
                    aes.IV = iv;
                }
                else
                {
                    // Fallback naar Optie B (Vaste lege IV voor legacy bestanden)
                    aes.IV = new byte[16];
                    cipherText = fileBytes;
                }

                // Voer de werkelijke AES-decryptie uit
                using (MemoryStream ms = new MemoryStream(cipherText))
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (StreamReader sr = new StreamReader(cs, Encoding.UTF8))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        catch (CryptographicException)
        {
            // SEC-15: Vang specifieke crypto-fouten op (bijv. verkeerd wachtwoord gebruikt)
            return "[Security] Onjuiste passphrase. De data is corrupt of onleesbaar.";
        }
        catch (Exception ex)
        {
            // SEC-15: Algemene foutafhandeling om crashes te voorkomen
            return $"[Fout] Er is een fout opgetreden tijdens het ontsleutelen: {ex.Message}";
        }
    }
}