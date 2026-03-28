namespace DungeonGame
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Console.Write(
                "═══════════════════════════════════════════\r\n" +
                "        DUNGEON OF NO RETURN\r\n" +
                "═══════════════════════════════════════════\r\n" +
                " You wake up in the dark. Cold stone beneath\r\n" +
                " your hands. Somewhere ahead — a growl." +
                "\r\n \r\n Find the treasure. Don't die trying.\r\n" +
                "═══════════════════════════════════════════\r\n" +
                " Commands: go [direction] | pick [item]\r\n" +
                "           fight | quit\r\n" +
                "═══════════════════════════════════════════\r\n" +
                " Enter your name, brave soul: "
            );

            string playerName = Console.ReadLine() ?? "DefaultPlayer";

            Game game = new(playerName);



            // TODO: Game Interface

            while (!game.IsGameOver())
            {
                GetRoomInfo(game.Player.CurrentRoom);
                MakeDecision(game);
            }


            if (game.Player.IsWinner)
            {
                Console.WriteLine($"{game.Player.Name} You Win !");
            }
            else
            {
                Console.WriteLine("You Lose !");
            }

              

        }

        private static void MakeDecision(Game game)
        {

            bool hasAliveMonster = game.Player.CurrentRoom.Monster is not null
                       && game.Player.CurrentRoom.Monster.IsAlive;
            

            Console.WriteLine("Available actions are: ");
            Console.WriteLine("Go [east south west north]");
            Console.WriteLine("Pick [Room Item Name]");

            if (hasAliveMonster)
            {
                Console.WriteLine("Fight");

            }

            Console.WriteLine("Quit");

            Console.WriteLine("Make a choice be smart ! There is no return way anymore !");
            Console.Write("What you want to do?: ");

            string decisions = Console.ReadLine() ?? "";

            Console.WriteLine();
            string[] splited = decisions.Split(" ");


            if (splited[0].ToLower() == "quit")
            {
                game.Player.Health = 0;
            }
            else if (splited[0].ToLower() == "fight" && hasAliveMonster)
            {
                bool fightResult = game.Fight();
                Console.WriteLine(fightResult ? "You defeated the monster!" : "You died fighting!");
            }
            else if (splited[0].ToLower() == "go" && splited.Length == 2)
            {
                bool moveResult = game.Move(splited[1]);
                Console.WriteLine(moveResult ? "You moved to " + game.Player.CurrentRoom.Name : "You can't go that way!");
                
            }
            else if (splited[0].ToLower() == "pick" && splited.Length == 2)
            {
                bool pickResult = game.PickUp(splited[1]);
                Console.WriteLine(pickResult ? "You picked up " + splited[1] : "You can't pick that up!");
            }
            else
            {
                Console.WriteLine("Unknown decision");
            }

            game.CheckWin();

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void GetRoomInfo(Room currentRom)
        {
            Console.WriteLine();
            Console.WriteLine("Current Room:");
            Console.WriteLine(currentRom.Name);
            Console.WriteLine();

            if (currentRom.Items.Any())
            {
                Console.WriteLine("Items available to pick are:");
                foreach (Item i in currentRom.Items)
                {
                    Console.WriteLine(i);
                }

                Console.WriteLine();
            }

            if (currentRom.Monster is not null && currentRom.Monster.IsAlive)
            {
                Console.WriteLine();
                Console.WriteLine("Monster in this Room:");

                Console.WriteLine(currentRom.Monster.Name);
                Console.WriteLine("HP: " + "" + currentRom.Monster.Health);
                Console.WriteLine();
            }

            Console.WriteLine("Possible Paths are:");
            foreach (KeyValuePair<string, Room> exits in currentRom.Exits)
            {
                Console.WriteLine(exits.Key);
            }
            Console.WriteLine();
        }

    }
}