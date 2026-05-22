using DungeonGame.Interfaces;
using DungeonGame.Services; // Zorg dat de juiste namespace voor IAuthService wordt gebruikt
using System.Net.Http.Headers;

namespace DungeonGame.Infrastructure
{
    /// <summary>
    /// Een HTTP-pijplijn handler die elk uitgaand verzoek onderschept om authenticatiegegevens toe te voegen.
    /// </summary>
    /// <remarks>
    /// Deze klasse implementeert SEC-12: Het automatisch toevoegen van de JWT-token aan elke HTTP-request.
    /// </remarks>
    public class AuthHandler : DelegatingHandler
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Initialiseert een nieuwe instantie van de <see cref="AuthHandler"/>.
        /// </summary>
        /// <param name="authService">De service waaruit de actuele JWT-token wordt opgehaald.</param>
        public AuthHandler(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Onderschept het HTTP-verzoek en voegt de 'Authorization: Bearer' header toe indien de gebruiker is ingelogd.
        /// </summary>
        /// <param name="request">Het uitgaande HTTP-verzoek.</param>
        /// <param name="cancellationToken">Token om de operatie eventueel te annuleren.</param>
        /// <returns>Het antwoord van de server (HttpResponseMessage).</returns>
        /// <remarks>
        /// SEC-12: Indien een token aanwezig is in het geheugen, wordt deze via de 'Bearer' scheme toegevoegd.
        /// SEC-15: Deze gecentraliseerde aanpak verkleint de kans op vergeten headers bij nieuwe API-implementaties.
        /// </remarks>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // SEC-12: Controleer of er een actieve sessie en een geldige token beschikbaar is.
            if (_authService.IsLoggedIn && !string.IsNullOrEmpty(_authService.Token))
            {
                // Voeg de JWT toe aan de headers van het verzoek.
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);
            }

            // SEC-15: Geef het verzoek door naar de volgende handler in de pijplijn of naar het netwerk[cite: 82].
            // Eventuele netwerkfouten (401, 403, 500) worden hierna door de aanroepende service opgevangen[cite: 87].
            return await base.SendAsync(request, cancellationToken);
        }
    }
}