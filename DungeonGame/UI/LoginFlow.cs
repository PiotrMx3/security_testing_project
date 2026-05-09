using DungeonGame.Interfaces;

namespace DungeonGame.UI
{
    /// <summary>
    /// Beheert de interactieve login-stroom in de console.
    /// </summary>
    /// <remarks>
    /// Deze klasse fungeert als de brug tussen de UI en de <see cref="IAuthService"/> om te voldoen aan SEC-11.
    /// </remarks>
    public static class LoginFlow
    {
        /// <summary>
        /// Start een interactieve loop die de speler dwingt in te loggen voordat het spel begint.
        /// </summary>
        /// <param name="authService">De authenticatie-dienst die de API-aanroepen verzorgt.</param>
        /// <returns>De geauthenticeerde <see cref="IAuthService"/> instantie.</returns>
        /// <remarks>
        /// Onderdeel van SEC-11: Het spel start pas nadat een geldige JWT is verkregen.
        /// </remarks>
        public static async Task<IAuthService> Execute(IAuthService authService)
        {
            bool authenticated = false;

            // Blijf proberen totdat de login succesvol is of de speler de app sluit.
            while (!authenticated)
            {
                Console.WriteLine("\n--- LOGIN VEREIST ---");
                Console.Write(" Gebruikersnaam: ");

                // SEC-15: We vangen null-input op om crashes te voorkomen.
                string username = Console.ReadLine() ?? "";

                Console.Write(" Wachtwoord: ");
                // SEC-15: Gebruik van een veilige methode om input te maskeren.
                string password = ReadPassword();

                // SEC-15: De login-poging wordt uitgevoerd. De AuthService handelt 
                // netwerkfouten (401/500) intern af zonder te crashen.
                authenticated = await authService.LoginAsync(username, password);

                if (!authenticated)
                {
                    // SEC-15: Toon een duidelijke, veilige foutmelding aan de gebruiker.
                    Console.WriteLine(" Onjuiste gegevens of server onbereikbaar. Probeer het opnieuw.");
                }
            }

            return authService;
        }

        /// <summary>
        /// Leest de invoer van de console terwijl de karakters worden gemaskeerd met sterretjes (*).
        /// </summary>
        /// <returns>Het ingevoerde wachtwoord als platte tekst.</returns>
        /// <remarks>
        /// SEC-15: Implementatie van 'secure input' om 'shoulder surfing' te voorkomen.
        /// </remarks>
        private static string ReadPassword()
        {
            string pass = "";
            ConsoleKeyInfo key;

            do
            {
                // Lees de toets zonder deze direct op het scherm te tonen.
                key = Console.ReadKey(true);

                // Verwerk normale karakters (geen Backspace of Enter).
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                // Ondersteun correcties via de backspace-toets.
                else if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    pass = pass[..^1];
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter); // Stop zodra de gebruiker op Enter drukt.

            Console.WriteLine();
            return pass;
        }
    }
}