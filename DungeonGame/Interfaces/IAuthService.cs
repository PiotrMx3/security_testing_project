using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DungeonGame.Interfaces
{
    /// <summary>
    /// Definieert de contractuele verplichtingen voor authenticatie en autorisatie binnen de game client.
    /// </summary>
    /// <remarks>
    /// Deze interface waarborgt dat de game voldoet aan de security-eisen voor gebruikersbeheer en JWT-verwerking.
    /// </remarks>
    public interface IAuthService
    {
        /// <summary>
        /// De opgeslagen JSON Web Token (JWT) voor de huidige sessie.
        /// </summary>
        /// <remarks>
        /// Voldoet aan SEC-12: De token wordt uitsluitend in het geheugen bewaard.
        /// </remarks>
        string? Token { get; }

        /// <summary>
        /// De unieke gebruikersnaam van de ingelogde speler.
        /// </summary>
        string? Username { get; }

        /// <summary>
        /// De toegekende rol van de gebruiker (bijv. 'Player' of 'Admin').
        /// </summary>
        /// <remarks>
        /// Voldoet aan SEC-14: Bepaalt of een gebruiker 'noclip' rechten heeft.
        /// </remarks>
        string? UserRole { get; }

        /// <summary>
        /// Geeft aan of de speler succesvol is geauthenticeerd bij de server.
        /// </summary>
        bool IsLoggedIn { get; }

        /// <summary>
        /// Start het authenticatieproces door inloggegevens naar de API te sturen.
        /// </summary>
        /// <param name="username">De gebruikersnaam van de speler.</param>
        /// <param name="password">Het ongehashte wachtwoord.</param>
        /// <returns>Een Task die true teruggeeft bij een succesvolle login.</returns>
        /// <remarks>
        /// Implementeert SEC-11: Login via POST-request naar /api/auth/login.
        /// </remarks>
        Task<bool> LoginAsync(string username, string password);

        /// <summary>
        /// Registreert een nieuwe speler in het systeem (SEC-11).
        /// </summary>
        Task<bool> RegisterAsync(string username, string email, string password);

        /// <summary>
        /// Configureert de interne HTTP-client voor communicatie met de beveiligde API-endpoints.
        /// </summary>
        /// <param name="client">De HttpClient instantie, bij voorkeur geconfigureerd met een AuthHandler.</param>
        /// <remarks>
        /// Ondersteunt SEC-12: Zorgt ervoor dat de JWT automatisch aan elk verzoek wordt toegevoegd.
        /// </remarks>
        void SetClient(HttpClient client);

        /// <summary>
        /// Beëindigt de huidige sessie en wist alle gevoelige gegevens uit het geheugen.
        /// </summary>
        /// <remarks>
        /// Onderdeel van SEC-15: Secure coding door sessie-data niet langer dan nodig te bewaren.
        /// </remarks>
        void Logout();
    }
}