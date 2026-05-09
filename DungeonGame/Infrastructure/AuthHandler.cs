using System.Net.Http.Headers;
using DungeonGame.Services;

namespace DungeonGame.Infrastructure
{
    public class AuthHandler : DelegatingHandler
    {
        private readonly IAuthService _authService;

        public AuthHandler(IAuthService authService)
        {
            _authService = authService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // SEC-12: Als de speler is ingelogd, voeg de Bearer token toe aan de header
            if (_authService.IsLoggedIn && !string.IsNullOrEmpty(_authService.Token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);
            }

            // SEC-15: Veilige afhandeling van het request zelf
            return await base.SendAsync(request, cancellationToken);
        }
    }
}