using Microsoft.Extensions.Configuration;
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
        public RoomUnlockService(
            IConfiguration configuration,
            HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        // Methode die controleert of een room unlocked mag worden
        public async Task<bool> UnlockRoomAsync(string roomId, string passphrase)
        {
            // Call naar de API om de keyshare van de room op te halen
            var response = await _httpClient
                .GetFromJsonAsync<KeyShareResponse>(
                    $"http://localhost:5234/api/rooms/{roomId}/keyshare");

            // Als API niets terugstuurt => unlock mislukt
            if (response == null)
                return false;

            // Verwachte hash ophalen uit appsettings.jsonSystem.Net.Http.HttpRequestException: 'Kan geen verbinding maken omdat de doelcomputer de verbinding actief heeft geweigerd. (localhost:5234)'

            var expectedHash = _configuration[$"ExpectedHash:{roomId}"];

            // Als er geen hash bestaat voor deze room => unlock mislukt
            if (string.IsNullOrWhiteSpace(expectedHash))
                return false;

            // Nieuwe hash genereren op basis van:
            // API keyshare + user passphrase
            var computedHash =
                HashService.ComputeHash(response.KeyShare, passphrase);

            // Vergelijken:
            // gegenereerde hash vs verwachte hash
            // true = correcte passphrase
            // false = foute passphrase
            return computedHash.Equals(
                expectedHash,
                StringComparison.OrdinalIgnoreCase);
        }
    }

    // Klasse die overeenkomt met de JSON response van de API
    public class KeyShareResponse
    {
        // Room identifier
        public string RoomId { get; set; } = "";

        // Keyshare afkomstig van API
        public string KeyShare { get; set; } = "";
    }
}