using System;
using System.Security.Cryptography;
using System.Text;

namespace DungeonGame.Security
{
    /// <summary>
    /// Service verantwoordelijk voor het genereren van cryptografische hashes en sleutels (SEC-13).
    /// </summary>
    public static class HashService
    {
        /// <summary>
        /// Berekent de rauwe bytes van de sleutel op basis van de keyshare en passphrase.
        /// </summary>
        /// <param name="keyShare">De keyshare afkomstig van de API.</param>
        /// <param name="passphrase">De door de speler ingevoerde passphrase.</param>
        /// <returns>Een byte-array van 32 bytes (256-bit), ideaal als sleutel voor AES-256 decryptie.</returns>
        public static byte[] ComputeAesKey(string keyShare, string passphrase)
        {
            string input = $"{keyShare}:{passphrase}";

            // SHA256.HashData levert altijd exact 32 bytes op
            return SHA256.HashData(Encoding.UTF8.GetBytes(input));
        }

        /// <summary>
        /// Bereken een hexadecimale string-representatie van de SHA256-hash.
        /// Wordt gebruikt om te valideren of het wachtwoord klopt met de verwachte hash in appsettings.json.
        /// </summary>
        /// <param name="keyShare">De keyshare afkomstig van de API.</param>
        /// <param name="passphrase">De door de speler ingevoerde passphrase.</param>
        /// <returns>Een leesbare hex-string (bijv. "A1B2C3...") van de berekende hash.</returns>
        /// <remarks>
        /// Onderdeel van SEC-13 (verificatie) en SEC-15 (veilige data-afhandeling).
        /// </remarks>
        public static string ComputeHash(string keyShare, string passphrase)
        {
            // We hergebruiken de methode van de collega om de bytes te genereren
            byte[] hashBytes = ComputeAesKey(keyShare, passphrase);

            // Convert.ToHexString() zet de rauwe bytes om naar een schone hoofdletter hex-string
            // Dankzij StringComparison.OrdinalIgnoreCase in je service maakt hoofdletter/kleine letter niet uit.
            return Convert.ToHexString(hashBytes);
        }
    }
}