using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.Security
{
    internal class EncryptRoomFileGenerator
    {
        public static void CreateEncryptedRoomFile()
        {
            string keyshare = "9F57924B";
            string passphrase = "GeheimKamer1";
            string plainText = "You found the hidden treasure chamber!";

            byte[] key = SHA256.HashData(
                Encoding.UTF8.GetBytes($"{keyshare}:{passphrase}")
            );

            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            using MemoryStream output = new MemoryStream();

            output.Write(aes.IV, 0, aes.IV.Length);

            using (CryptoStream cryptoStream = new CryptoStream(
                output,
                aes.CreateEncryptor(),
                CryptoStreamMode.Write))
            {
                cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                cryptoStream.FlushFinalBlock();
            }

            Directory.CreateDirectory("EncryptedRooms");

            File.WriteAllBytes(
                "EncryptedRooms/room_treasure.enc",
                output.ToArray()
            );
        }
    }
}
