using DungeonGame.Interfaces;
using System.Net.Http.Json;

namespace DungeonGame.Services
{
    /// <summary>
    /// De implementatie van de authenticatie-dienst. 
    /// Verantwoordelijk voor het inloggen van spelers en het veilig beheren van de JWT-sessie in het geheugen.
    /// </summary>
    public class AuthService : IAuthService
    {
        private HttpClient? _httpClient;

        /// <summary>
        /// SEC-12: De JSON Web Token (JWT) die wordt gebruikt voor geauthenticeerde verzoeken. 
        /// Wordt uitsluitend in het geheugen bewaard.
        /// </summary>
        public string? Token { get; private set; }

        /// <summary>
        /// De gebruikersnaam van de momenteel ingelogde speler.
        /// </summary>
        public string? Username { get; private set; }

        /// <summary>
        /// SEC-14: De rol van de gebruiker (bijv. 'Player' of 'Admin'). 
        /// Bepaalt toegangsrechten zoals 'noclip'.
        /// </summary>
        public string? UserRole { get; private set; }

        /// <summary>
        /// Geeft aan of er een actieve, geauthenticeerde sessie is.
        /// </summary>
        public bool IsLoggedIn => !string.IsNullOrEmpty(Token);

        /// <summary>
        /// Ontwikkelvlag om te kunnen testen zonder dat de Minimal API (Block 1) draait.
        /// </summary>
        private bool _useMock = true;

        /// <summary>
        /// Initialiseert een nieuwe instantie van de <see cref="AuthService"/>.
        /// </summary>
        /// <param name="httpClient">Optionele HttpClient; kan later via SetClient worden gezet om circulaire afhankelijkheden te voorkomen.</param>
        public AuthService(HttpClient? httpClient = null)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// SEC-12: Configureert de HttpClient voor deze service. 
        /// Nodig voor de koppeling met de AuthHandler.
        /// </summary>
        /// <param name="client">De geconfigureerde HttpClient.</param>
        public void SetClient(HttpClient client)
        {
            _httpClient = client;
        }

        /// <summary>
        /// SEC-11: Handelt het inlogproces af via de API.
        /// </summary>
        /// <param name="username">De ingevoerde gebruikersnaam.</param>
        /// <param name="password">Het ongehashte wachtwoord.</param>
        /// <returns>True als de login succesvol is en de JWT is opgeslagen; anders false.</returns>
        public async Task<bool> LoginAsync(string username, string password)
        {
            // SEC-15: Input-validatie door middel van trimming om onbedoelde spaties te verwijderen.
            username = username.Trim();
            password = password.Trim();

            if (_useMock)
            {
                // Tijdelijke simulatie van een succesvolle login (SEC-11) voor ontwikkeldoeleinden. 
                if (username == "admin" && password == "admin123")
                {
                    Token = "mock-jwt-token-admin";
                    Username = username;
                    UserRole = "Admin"; // Maakt Admin noclip (SEC-14) testbaar.
                    return true;
                }
                return false;
            }

            try
            {
                // SEC-15: Controle op aanwezigheid van HttpClient om runtime-crashes te voorkomen.
                if (_httpClient == null) throw new Exception("HttpClient niet geconfigureerd.");

                // SEC-11: POST-verzoek naar de API. Het wachtwoord wordt op de server gehashed met SHA-256.
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { username, password });

                if (response.IsSuccessStatusCode)
                {
                    // SEC-11 & SEC-12: JWT en gebruikersinformatie ophalen en veilig in-memory opslaan.
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    Token = result?.Token;
                    UserRole = result?.Role;
                    Username = username;
                    return true;
                }

                // Bij een fout (bijv. 401 Unauthorized of Lockout) geven we simpelweg false terug.
                return false;
            }
            catch (Exception)
            {
                // SEC-15: Secure coding — fouten tijdens API-communicatie worden opgevangen zonder dat de game crasht.
                return false;
            }
        }

        /// <summary>
        /// Verwijdert de huidige sessiegegevens en logt de speler uit.
        /// </summary>
        public void Logout()
        {
            Token = null;
            Username = null;
            UserRole = null;
        }
    }

    /// <summary>
    /// Data Transfer Object (DTO) voor het verwerken van het API-antwoord bij login. 
    /// </summary>
    public class LoginResponse
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
    }
}