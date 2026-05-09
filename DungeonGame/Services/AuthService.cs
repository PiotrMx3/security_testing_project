using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.Services
{
    public class AuthService : IAuthService
    {
        private HttpClient? _httpClient;
        public string? Token { get; private set; }
        public string? Username { get; private set; }
        public string? UserRole { get; private set; }
        public bool IsLoggedIn => !string.IsNullOrEmpty(Token);

        private bool _useMock = true;

        // Constructor accepteert nu 'null' om de cirkel-dependency te verbreken
        public AuthService(HttpClient? httpClient = null)
        {
            _httpClient = httpClient;
        }

        // SEC-12: Voeg deze methode toe
        public void SetClient(HttpClient client)
        {
            _httpClient = client;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            username = username.Trim();
            password = password.Trim();

            if (_useMock)
            {
                // Tijdelijke mock voor SEC-11
                if (username == "admin" && password == "admin123")
                {
                    Token = "mock-jwt-token-admin";
                    Username = username;
                    UserRole = "Admin";
                    return true;
                }
                return false;
            }

            try
            {
                if (_httpClient == null) throw new Exception("HttpClient niet geconfigureerd.");

                var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { username, password });

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    Token = result?.Token;
                    UserRole = result?.Role;
                    Username = username;
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                // SEC-15: Foutmelding tonen zonder crash [cite: 45, 87]
                return false;
            }
        }

        public void Logout() { /* ... reset fields ... */ }
    }

    // Hulpklasse voor het antwoord van de API
    public class LoginResponse
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
    }
}