namespace DungeonGame
{
    public class Game
    {
        public Player Player { get; private set; }

        private Room entrance;
        private Room armory;
        private Room crypt;
        private Room pillarHall;
        private Room monsterRoom;
        private Room treasureRoom;

        // Constructor for Program.cs
        public Game(string playerName)
        {
            SetupRooms();
            Player = new Player(playerName, 100, entrance);
        }

        // Constructor for Testing

        public Game(Player player, Room entrance, Room armory, Room crypt,
                    Room pillarHall, Room monsterRoom, Room treasureRoom)
        {
            Player = player;
            this.entrance = entrance;
            this.armory = armory;
            this.crypt = crypt;
            this.pillarHall = pillarHall;
            this.monsterRoom = monsterRoom;
            this.treasureRoom = treasureRoom;
        }

        private void SetupRooms()
        {
            // Rooms
            entrance = new Room("Entrance", "A dark doorway.");
            armory = new Room("Armory", "Weapons on the walls.");
            crypt = new Room("Crypt", "Cold and silent.");
            pillarHall = new Room("PillarHall", "Tall stone pillars.", hasTrap: true, trapDamage: 10);
            monsterRoom = new Room("MonsterRoom", "You hear growling.");
            treasureRoom = new Room("TreasureRoom", "Glittering gold.", isLocked: true, requiredKeyName: "GoldenKey");

            // Rooms Exits
            entrance.AddExit("east", armory);
            entrance.AddExit("south", pillarHall);

            armory.AddExit("west", entrance);
            armory.AddExit("east", crypt);

            crypt.AddExit("west", armory);
            crypt.AddExit("south", monsterRoom);

            pillarHall.AddExit("north", entrance);
            pillarHall.AddExit("east", monsterRoom);

            monsterRoom.AddExit("south", treasureRoom);

            // Room Items
            entrance.Items.Add(new Item("Torch", "Lights the way.", ItemType.Consumable));
            entrance.Items.Add(new Item("OldMap", "A worn map.", ItemType.Consumable));

            armory.Items.Add(new Item("Sword", "A sharp blade.", ItemType.Weapon));
            armory.Items.Add(new Item("Shield", "Sturdy protection.", ItemType.Consumable));

            crypt.Items.Add(new Item("GoldenKey", "Opens the treasure room.", ItemType.Key));

            pillarHall.Items.Add(new Item("HealthPotion", "Restores 20 HP.", ItemType.Consumable));

            treasureRoom.Items.Add(new Item("Treasure", "You win!", ItemType.Consumable));

            // Monsters
            armory.Monster = new Monster("Skeleton", 30, 10);
            crypt.Monster = new Monster("Ghost", 20, 15);
            monsterRoom.Monster = new Monster("Troll", 50, 25, requiresWeapon: true);
        }


        public bool Move(string direction)
        {
            return Player.Move(direction);

        }


        public bool PickUp(string itemName)
        {
            Item? item = Player.CurrentRoom.Items.FirstOrDefault(i => i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));

            if (item is not null) return Player.PickUpItem(item);

            return false;

        }

        public bool Fight()
        {


            if (Player.CurrentRoom.Monster is null)
            {
                return false;
            }

            if (Player.CurrentRoom.Monster.RequiresWeapon && !Player.Inventory.HasWeapon())
            {
                Player.Health = 0;
                return false;
            }


            while (Player.CurrentRoom.Monster.IsAlive && Player.IsAlive)
            {
                bool rng = new Random().Next(0, 100) < 25;
                int rDamage = rng ? 5 : 20;

                Player.CurrentRoom.Monster.TakeDamage(rDamage);

                if (!Player.CurrentRoom.Monster.IsAlive) return true;

                Player.CurrentRoom.Monster.Attack(Player);

                if (!Player.IsAlive) return false;

            }

            // Faalback you never know :)

            return false;

        }



        public bool IsGameOver()
        {
            if (!Player.IsAlive || Player.IsWinner) return true;

            return false;
        }

        public bool CheckWin()
        {
            
            if(Player.CurrentRoom.Name.Equals("TreasureRoom") && Player.Inventory.Contains("Treasure"))
            {
                Player.IsWinner = true;
                return true;
            }

            return false;
        }

    }

}
