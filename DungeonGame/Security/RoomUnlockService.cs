using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace DungeonGame.Security
{
    public class RoomUnlockService
    {
        // Hiermee lezen we waarden uit appsettings.json
        private readonly IConfiguration _configuration;

        // Hiermee maken we HTTP-calls naar de API
        private readonly HttpClient _httpClient;

        // Constructor krijgt configuration en HttpClient binnen
        public RoomUnlockService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        // Methode die controleert of een room unlocked mag worden +jwt token controle
        public async Task<string> DecryptRoomAsync(string roomId, string encryptedFilePath, string passphrase, string jwtToken)
        {
            // 1. JWT meegeven
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwtToken);

            // 2. Keyshare ophalen via API
            var response = await _httpClient
                .GetFromJsonAsync<KeyShareResponse>(
                    $"https://localhost:7100/api/rooms/{roomId}/keyshare");

            if (response == null || string.IsNullOrWhiteSpace(response.KeyShare))
                throw new InvalidOperationException("Keyshare kon niet opgehaald worden.");

            // 3. AES-key maken
            byte[] aesKey = HashService.ComputeAesKey(response.KeyShare, passphrase);

            // 4. .enc-bestand lezen
            byte[] encryptedData = await File.ReadAllBytesAsync(encryptedFilePath);

            // 5. Decrypt proberen
            // Bij verkeerde passphrase gooit deze normaal een CryptographicException
            return AesEncryptionService.Decrypt(encryptedData, aesKey);
        }
        public async Task<string> LoginAsync(string userName, string password)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "https://localhost:7100/account/login",
                new
                {
                    userName,
                    password
                });

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            return result?.Token ?? throw new InvalidOperationException("Geen token ontvangen.");
        }

        public async Task RegisterAsync(string userName, string email, string password)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "https://localhost:7100/account/register",
                new
                {
                    userName,
                    email,
                    password
                });

        }


        // Klasse die overeenkomt met de JSON response van de API
        public class KeyShareResponse
        {
            // Room identifier
            public string RoomId { get; set; } = "";

            // Keyshare afkomstig van API
            public string KeyShare { get; set; } = "";
        }

        public class LoginResponse
        {
            public string Token { get; set; } = "";
        }
    }

}