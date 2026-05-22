using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace DungeonGame.Security
{
    /// <summary>
    /// Service die verantwoordelijk is voor het controleren van kamersleutels bij de API en het initiëren van decryptie.
    /// </summary>
    public class RoomUnlockService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initialiseert een nieuwe instantie van de <see cref="RoomUnlockService"/> klasse.
        /// </summary>
        /// <param name="configuration">De configuratie om de verwachte hashes uit te lezen.</param>
        /// <param name="httpClient">De (beveiligde) HTTP-client voor API-communicatie.</param>
        public RoomUnlockService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        /// <summary>
        /// SEC-13/15: Haalt de keyshare op via de beveiligde API, controleert de passphrase via een hash,
        /// en ontsleutelt het lokale .enc bestand bij succes.
        /// </summary>
        /// <param name="roomId">Het unieke ID van de kamer.</param>
        /// <param name="passphrase">De door de speler ingevoerde passphrase.</param>
        /// <returns>De ontsleutelde omschrijving van de kamer, of null als de toegang geweigerd is.</returns>
        public async Task<string?> UnlockAndDecryptRoomAsync(string roomId, string passphrase)
        {
            try
            {
                // SEC-12: Relatieve URL zodat de call via BaseAddress én de AuthHandler (JWT) loopt
                var response = await _httpClient.GetFromJsonAsync<KeyShareResponse>($"api/rooms/{roomId}/keyshare");

                if (response == null || string.IsNullOrWhiteSpace(response.KeyShare))
                    return null;

                // Haal de verwachte hash op uit appsettings.json van de client
                var expectedHash = _configuration[$"ExpectedHash:{roomId}"];
                if (string.IsNullOrWhiteSpace(expectedHash))
                    return null;

                // Genereer de controle-hash op basis van de API-keyshare en gebruikersinput
                var computedHash = HashService.ComputeHash(response.KeyShare, passphrase);

                // Vergelijk de hashes om te zien of het wachtwoord correct is
                if (computedHash.Equals(expectedHash, StringComparison.OrdinalIgnoreCase))
                {
                    // SEC-13: Wachtwoord is correct! Roep de DecryptionService aan om het lokale .enc bestand te kraken
                    string decryptedContent = DecryptionService.DecryptRoomFile(roomId, response.KeyShare, passphrase);
                    return decryptedContent;
                }

                return null; // Wachtwoord was fout
            }
            catch (HttpRequestException)
            {
                // SEC-15: Server offline of 401/403/500 error opvangen zonder dat de game crasht
                Console.WriteLine("[Netwerk Fout] Kan geen veilige verbinding maken met de sleutel-server.");
                return null;
            }
            catch (Exception ex)
            {
                // SEC-15: Catch-all om onverwachte runtime crashes te voorkomen
                Console.WriteLine($"[Fout] Probleem bij het ontgrendelen: {ex.Message}");
                return null;
            }
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

    /// <summary>
    /// Hulpklasse (Data Transfer Object) die overeenkomt met de JSON-response van de GetRoomKeyShare API.
    /// </summary>
    public class KeyShareResponse
    {
        /// <summary>Het unieke ID van de opgevraagde kamer.</summary>
        public string RoomId { get; set; } = "";

        /// <summary>De cryptografische keyshare afkomstig van de database/API.</summary>
        public string KeyShare { get; set; } = "";
    }
}