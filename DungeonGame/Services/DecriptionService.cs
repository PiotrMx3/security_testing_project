using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class DecryptionService
{
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

                // NOTITIE OVER DE IV (Initialization Vector):
                // Optie A: De IV staat in de eerste 16 bytes van het .enc bestand (Meest professioneel)
                // Optie B: Er is een vaste IV gebruikt van 16 nullen.

                byte[] iv = new byte[16];
                byte[] cipherText;

                if (fileBytes.Length > 16)
                {
                    // We gaan uit van Optie A: IV herleiden uit het bestand
                    Array.Copy(fileBytes, 0, iv, 0, 16);
                    cipherText = new byte[fileBytes.Length - 16];
                    Array.Copy(fileBytes, 16, cipherText, 0, cipherText.Length);
                    aes.IV = iv;
                }
                else
                {
                    // Fallback naar Optie B (Vaste lege IV)
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