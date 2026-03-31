namespace DungeonGame
{
    public class Game
    {
        public IPlayer Player { get; private set; }
        public Rooms Rooms { get; private set; }

        // Constructor for Program.cs
        public Game(string playerName)
        {
            Rooms = GameSetup.CreateWorld();
            Player = new Player(playerName, 100);
        }

        // Constructor for Testing
        public Game(IPlayer player, Rooms rooms)
        {
            Player = player;
            Rooms = rooms;
        }

        public bool Move(string directionInput)
        {
            Direction? direction = DirectionHelper.Parse(directionInput);

            if (direction == null)
                return false;

            return Rooms.Move(direction.Value, Player);
        }

        public bool Take(string itemName)
        {
            Item? item = Rooms.CurrentRoom.TakeItem(itemName);

            if (item == null)
                return false;

            if (!Player.Inventory.Add(item))
            {
                // Item didn't fit in inventory, put it back in the room
                Rooms.CurrentRoom.Items.Add(item);
                return false;
            }

            return true;
        }

        public bool Fight()
        {
            return Rooms.Fight(Player);
        }

        public string Look()
        {
            string info = Rooms.CurrentRoom.Describe();

            info += "\nYour inventory:\n";
            if (Player.Inventory.Items.Any())
            {
                foreach (Item item in Player.Inventory.Items)
                {
                    info += $"  - {item}\n";
                }
            }
            else
            {
                info += "  (empty)\n";
            }

            info += $"\nHP: {Player.Health}\n";

            return info;
        }

        public string ShowInventory()
        {
            string info = "Inventory:\n";

            if (Player.Inventory.Items.Any())
            {
                foreach (Item item in Player.Inventory.Items)
                {
                    info += $"  - {item}\n";
                }
            }
            else
            {
                info += "  (empty)\n";
            }

            return info;
        }

        public string Help()
        {
            string info = "";
            info += "Available commands:\n";
            info += "  help              - show this list\n";
            info += "  look              - show room info, items, exits and inventory\n";
            info += "  inventory         - show your inventory\n";
            info += "  go n|e|s|w        - move to another room\n";
            info += "  take <item name>  - pick up an item\n";
            info += "  fight             - fight the monster in this room\n";
            info += "  quit              - stop the game\n";
            return info;
        }

        public bool CheckWin()
        {
            if (Rooms.CurrentRoom.Name == "WinRoom")
            {
                Player.IsWinner = true;
                return true;
            }

            return false;
        }

        public bool IsGameOver()
        {
            if (!Player.IsAlive || Player.IsWinner) return true;

            return false;
        }
    }
}
