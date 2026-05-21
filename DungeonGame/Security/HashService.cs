using System.Text;
using System.Security.Cryptography;

namespace DungeonGame.Security
{
    public static class HashService
    {
        public static byte[] ComputeAesKey(string keyShare, string passphrase)
        {
            string input = $"{keyShare}:{passphrase}";

            return SHA256.HashData(Encoding.UTF8.GetBytes(input));
        }
    }
}
