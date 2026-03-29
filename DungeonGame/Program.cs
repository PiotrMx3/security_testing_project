namespace DungeonGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
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

            Console.Write(" Enter your name, brave soul: ");
            string playerName = Console.ReadLine() ?? "DefaultPlayer";

            Game game = new Game(playerName);

            Console.WriteLine($"\nWelcome, {playerName}! Type 'help' for commands.\n");

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
