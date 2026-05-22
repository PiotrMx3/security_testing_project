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
                Console.WriteLine($"\n--- [DEBUG START] ONTSLEUTELING VOOR: {roomId} ---");

                // 1. Controleer de API-verbinding en keyshare
                var response = await _httpClient.GetFromJsonAsync<KeyShareResponse>($"api/rooms/{roomId}/keyshare");

                if (response == null || string.IsNullOrWhiteSpace(response.KeyShare))
                {
                    Console.WriteLine("[DEBUG] FOUT: De API stuurde een lege of ongeldige Keyshare terug!");
                    return null;
                }
                Console.WriteLine($"[DEBUG] Keyshare ontvangen van API: '{response.KeyShare}'");

                // 2. Controleer het inlezen van appsettings.json
                var expectedHash = _configuration[$"ExpectedHash:{roomId}"];
                Console.WriteLine($"[DEBUG] Verwachte hash uit appsettings:  '{expectedHash}'");

                if (string.IsNullOrWhiteSpace(expectedHash))
                {
                    Console.WriteLine($"[DEBUG] FOUT: De sleutel 'ExpectedHash:{roomId}' kon NIET worden gevonden in appsettings.json! Bestaat het bestand wel in de build-map?");
                    return null;
                }

                // 3. Controleer de berekende hash van de speler
                var computedHash = HashService.ComputeHash(response.KeyShare, passphrase);
                Console.WriteLine($"[DEBUG] Jouw berekende hash lokaal:       '{computedHash}'");

                // 4. De vergelijking
                if (computedHash.Equals(expectedHash, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("[DEBUG] MATCH! Hashes zijn gelijk. Starten van decryptie...");
                    string decryptedContent = AesEncryptionService.DecryptRoomFile(roomId, response.KeyShare, passphrase);
                    Console.WriteLine($"[DEBUG] Resultaat uit decryptor: '{decryptedContent}'");
                    return decryptedContent;
                }

                Console.WriteLine("[DEBUG] FOUT: De berekende hash en verwachte hash matchen NIET!");
                Console.WriteLine("--- [DEBUG EIND] ---\n");
                return null;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"[Netwerk Fout] Kan geen veilige verbinding maken met de sleutel-server: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
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
}