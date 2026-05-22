using DungeonGame.Interfaces;
using System;
using System.Threading.Tasks;

namespace DungeonGame.UI
{
    /// <summary>
    /// Beheert de interactieve authenticatie- en registratiestroom binnen de console-interface.
    /// </summary>
    /// <remarks>
    /// Deze klasse fungeert als de gebruikersinterface-laag voor accountbeheer en dwingt af dat er een 
    /// geldige actieve sessie (JWT) is voordat de speler de dungeon mag betreden, conform eis SEC-11.
    /// </remarks>
    public static class LoginFlow
    {
        /// <summary>
        /// Start de centrale keuzeloop waarin de speler verplicht wordt om in te loggen of te registreren.
        /// </summary>
        /// <param name="authService">De authenticatiedienst die verantwoordelijk is voor de communicatie met de API endpoints.</param>
        /// <returns>De bijgewerkte instantie van de <see cref="IAuthService"/> die de actieve JWT-token bevat.</returns>
        /// <remarks>
        /// Onderdeel van SEC-11: Het spel start pas definitief wanneer deze loop succesvol wordt doorbroken 
        /// door een geslaagde inlogpoging.
        /// </remarks>
        public static async Task<IAuthService> Execute(IAuthService authService)
        {
            Console.WriteLine("\n=== WELCOME TO THE DUNGEON MANAGEMENT SYSTEM ===");

            // Blijf in de loop zolang de speler niet succesvol is ingelogd
            while (!authService.IsLoggedIn)
            {
                Console.WriteLine("\n[1] Login as existing player");
                Console.WriteLine("[2] Register a new account");
                Console.WriteLine("[3] Exit Game");
                Console.Write("\nChoose an option (1-3): ");

                string choice = Console.ReadLine() ?? "";

                switch (choice.Trim())
                {
                    case "1":
                        await HandleLogin(authService);
                        break;
                    case "2":
                        await HandleRegister(authService);
                        break;
                    case "3":
                        Console.WriteLine("Exiting game. Safe travels!");
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("[Input Error] Invalid option. Please type 1, 2 or 3.");
                        break;
                }
            }

            return authService;
        }

        /// <summary>
        /// Handelt het interactief opvragen van inloggegevens af en voert de authenticatie uit via de API.
        /// </summary>
        /// <param name="authService">De authenticatiedienst die wordt aangeroepen om de credentials te verifiëren.</param>
        /// <returns>Een <see cref="Task"/> die de asynchrone operatie vertegenwoordigt.</returns>
        /// <remarks>
        /// Voldoet aan SEC-15 door de invoer veilig te verwerken (null-checks) en het wachtwoord te maskeren tijdens het typen.
        /// </remarks>
        private static async Task HandleLogin(IAuthService authService)
        {
            Console.WriteLine("\n--- PLAYER LOGIN ---");
            Console.Write(" Username: ");
            string username = Console.ReadLine() ?? "";

            Console.Write(" Password: ");
            string password = ReadPassword();

            // SEC-11: Voer de POST-request uit naar de API (wordt intern door de service afgehandeld)
            bool authenticated = await authService.LoginAsync(username, password);

            if (authenticated)
            {
                Console.WriteLine("\n>>> Login successful! Loading player data...");
            }
            else
            {
                // SEC-15: Nette foutmelding in plaats van een applicatiecrash bij foutieve invoer of serverfouten
                Console.WriteLine("\n[Error] Access denied. Invalid username/password or server offline.");
            }
        }

        /// <summary>
        /// Handelt het interactief opvragen van registratiegegevens af en maakt een nieuw account aan via de API.
        /// </summary>
        /// <param name="authService">De authenticatiedienst die de registratie-request naar de backend stuurt.</param>
        /// <returns>Een <see cref="Task"/> die de asynchrone operatie vertegenwoordigt.</returns>
        /// <remarks>
        /// Voldoet aan SEC-15 door directe validatie toe te passen op lege velden alvorens de API onnodig te belasten.
        /// </remarks>
        private static async Task HandleRegister(IAuthService authService)
        {
            Console.WriteLine("\n--- REGISTER NEW ACCOUNT ---");
            Console.Write(" Desired Username: ");
            string username = Console.ReadLine() ?? "";

            Console.Write(" Email Address: ");
            string email = Console.ReadLine() ?? "";

            Console.Write(" Password (min. 8 characters, 1 uppercase, 1 digit): ");
            string password = ReadPassword();

            // SEC-15: Lokale input-validatie om loze netwerkverzoeken te voorkomen
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("\n[Input Error] Username and password fields cannot be empty.");
                return;
            }

            // SEC-11: Verstuur de registratiegegevens naar het account/register endpoint
            bool success = await authService.RegisterAsync(username, email, password);

            if (success)
            {
                Console.WriteLine("\n>>> Registration successful! You can now log in using option 1.");
            }
            else
            {
                // SEC-15: Veilige foutmelding die eventuele server- en databasefouten maskeert voor de eindgebruiker
                Console.WriteLine("\n[Error] Registration failed. Username may be taken or password does not meet requirements.");
            }
        }

        /// <summary>
        /// Leest invoer van de console waarbij de ingetypte karakters direct visueel gemaskeerd worden met sterretjes (*).
        /// </summary>
        /// <returns>Het daadwerkelijk ingevoerde wachtwoord als een onbewerkte string.</returns>
        /// <remarks>
        /// Voldoet aan SEC-15 door 'shoulder surfing' tegen te gaan en biedt volledige ondersteuning voor correcties via de Backspace-toets.
        /// </remarks>
        private static string ReadPassword()
        {
            string pass = "";
            ConsoleKeyInfo key;

            do
            {
                // Lees de toetsaanslag in zonder deze direct op het scherm te echoën
                key = Console.ReadKey(true);

                // Filter speciale functionele toetsen zoals Enter of Backspace uit voor reguliere invoer
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                // Ondersteun actieve correcties als de gebruiker een typefout maakt
                else if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    pass = pass[..^1];
                    Console.Write("\b \b"); // Verplaats de cursor terug, overschrijf met een spatie, en zet de cursor weer terug
                }
            } while (key.Key != ConsoleKey.Enter); // Beëindig de invoer zodra de gebruiker op Enter drukt

            Console.WriteLine();
            return pass;
        }
    }
}