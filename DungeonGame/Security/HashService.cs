using System.Text;
using System.Security.Cryptography;

namespace DungeonGame.Security
{
    public static class HashService
    {
        public static string ComputeHash(string keyShare, string passphrase)
        {
            var input = $"{keyShare}:{passphrase}";

            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

            return Convert.ToHexString(bytes);
        }
    }
}
