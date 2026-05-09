using DungeonGame.Infrastructure;
using DungeonGame.Services;
using DungeonGame.UI;
using System.Xml.Linq;

namespace DungeonGame
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // 1. Maak eerst de service aan (zonder HttpClient)
            var authService = new AuthService();

            // 2. Maak de AuthHandler en koppel deze aan de service
            var authHandler = new AuthHandler(authService)
            {
                InnerHandler = new HttpClientHandler() // De standaard handler die het echte werk doet
            };

            // 3. Maak de HttpClient die ALTIJD via jouw authHandler gaat
            var httpClient = new HttpClient(authHandler)
            {
                BaseAddress = new Uri("http://localhost:5000/")
            };

            // 4. Geef de httpClient nu pas aan de authService (Dependency Injection)
            // Je kunt een kleine methode toevoegen aan AuthService om de client later te zetten, 
            // of de constructor aanpassen.
            authService.SetClient(httpClient);

            Console.WriteLine(
                "═══════════════════════════════════════════\n" +
                "        DUNGEON OF NO RETURN\n" +
                "═══════════════════════════════════════════\n" +
                " You wake up in the dark. Cold stone beneath\n" +
                " your hands. Somewhere ahead — a growl.\n" +
                "\n" +
                " Find the treasure. Don't die trying.\n" +
                "═══════════════════════════════════════════\n" +
                " Commands: help | look | inventory\n" +
                "           go n|e|s|w | take <item>\n" +
                "           fight | quit\n" +
                "═══════════════════════════════════════════");

            IAuthService finalAuth = await LoginFlow.Execute(authService);

            Game game = new Game(finalAuth.Username ?? "Player", finalAuth);

            Console.WriteLine($"\nWelcome, {finalAuth.Username}! Type 'help' for commands.\n");
            
            while (!game.IsGameOver())
            {
                Console.Write("> ");
                string input = Console.ReadLine() ?? "";
                Console.WriteLine();

                HandleCommand(input, game);

                game.CheckWin();
            }

            if (game.Player.IsWinner)
            {
                Console.WriteLine($"\n{game.Player.Name}, you escaped the dungeon! You win!");
            }
            else
            {
                Console.WriteLine("\nYou died. Game over!");
            }
        }

        private static void HandleCommand(string input, Game game)
        {
            string[] parts = input.Trim().Split(" ", 2);
            string command = parts[0].ToLower();

            if (command == "help")
            {
                Console.WriteLine(game.Help());
            }
            else if (command == "look")
            {
                Console.WriteLine(game.Look());
            }
            else if (command == "inventory")
            {
                Console.WriteLine(game.ShowInventory());
            }
            else if (command == "go" && parts.Length == 2)
            {
                bool moved = game.Move(parts[1]);

                if (!game.Player.IsAlive)
                {
                    Console.WriteLine("You entered a deadly room...");
                }
                else if (moved)
                {
                    Console.WriteLine("You moved to " + game.Rooms.CurrentRoom.Name);
                }
                else
                {
                    Console.WriteLine("You can't go that way!");
                }
            }
            else if (command == "take" && parts.Length == 2)
            {
                bool picked = game.Take(parts[1]);

                if (picked)
                {
                    Console.WriteLine("You picked up " + parts[1]);
                }
                else
                {
                    Console.WriteLine("You can't take that!");
                }
            }
            else if (command == "fight")
            {
                bool hasMonster = game.Rooms.CurrentRoom.Monster != null
                                  && game.Rooms.CurrentRoom.Monster.IsAlive;

                if (!hasMonster)
                {
                    Console.WriteLine("There is nothing to fight here.");
                }
                else
                {
                    bool won = game.Fight();
                    Console.WriteLine(won ? "You defeated the monster!" : "You died fighting!");
                }
            }
            else if (command == "quit")
            {
                game.Player.Health = 0;
                Console.WriteLine("You gave up...");
            }
            else
            {
                Console.WriteLine("Unknown command. Type 'help' for a list of commands.");
            }
        }
    }
}
