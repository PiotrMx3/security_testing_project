using System.Security.Cryptography;
using System.Text;

namespace DungeonGame.Security
{
    internal class AesEncryptionService
    {
        public static string Decrypt(byte[] encryptedData, byte[] key)
        {
            using Aes aes = Aes.Create();
            aes.Key = key;

            byte[] iv = encryptedData.Take(16).ToArray();
            byte[] cipherText = encryptedData.Skip(16).ToArray();

            aes.IV = iv;

            using MemoryStream input = new MemoryStream(cipherText);
            using CryptoStream cryptoStream = new CryptoStream(
                input,
                aes.CreateDecryptor(),
                CryptoStreamMode.Read);

            using StreamReader reader = new StreamReader(cryptoStream, Encoding.UTF8);

            return reader.ReadToEnd();
        }
    }
}
