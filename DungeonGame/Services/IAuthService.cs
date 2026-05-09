using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.Services
{
    public interface IAuthService
    {
        string? Token { get; }
        string? Username { get; }
        string? UserRole { get; }
        bool IsLoggedIn { get; }

        Task<bool> LoginAsync(string username, string password);
        // SEC-12: Methode om de geconfigureerde HttpClient later te injecteren
        void SetClient(HttpClient client);
        void Logout();
    }
}
