namespace DungeonGame
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine(
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
                " Enter your name, brave soul:");

            string playerName = Console.ReadLine() ?? "DefaultPlayer";

            Game game = new(playerName);



            // TODO: Game Interface

            //while (!game.IsGameOver())
            //{

            //}




        }
    }
}