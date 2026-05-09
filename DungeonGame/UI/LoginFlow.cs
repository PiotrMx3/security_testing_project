using DungeonGame.Services;

namespace DungeonGame.UI
{
    public static class LoginFlow
    {
        public static async Task<IAuthService> Execute(IAuthService authService)
        {
            bool authenticated = false;

            while (!authenticated)
            {
                Console.WriteLine("\n--- LOGIN VEREIST ---");
                Console.Write(" Gebruikersnaam: ");
                string username = Console.ReadLine() ?? "";

                Console.Write(" Wachtwoord: ");
                string password = ReadPassword();

                // SEC-15: Aanroep naar de service met foutafhandeling binnenin
                authenticated = await authService.LoginAsync(username, password);

                if (!authenticated)
                {
                    Console.WriteLine(" Onjuiste gegevens of server onbereikbaar. Probeer het opnieuw.");
                }
            }

            return authService;
        }

        // De hulpmethode om het wachtwoord veilig te lezen met sterretjes
        private static string ReadPassword()
        {
            string pass = "";
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    pass = pass[..^1];
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();
            return pass;
        }
    }
}