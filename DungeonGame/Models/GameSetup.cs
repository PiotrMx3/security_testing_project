namespace DungeonGame
{
    public class GameSetup
    {
        public static Rooms CreateWorld()
        {
            // Rooms
            Room start = new Room("Start", "You are in a dark room. Exits lead in all directions.");
            Room deathRoom = new Room("DeathRoom", "The floor collapses beneath you!", isDeadly: true);
            Room keyRoom = new Room("KeyRoom", "A small dusty room. Something shines in the corner.");
            Room winRoom = new Room("WinRoom", "A bright room with a golden door. You made it!", isLocked: true, requiredKeyName: "Key");
            Room swordRoom = new Room("SwordRoom", "An old armory. Weapons hang on the walls.");
            Room monsterRoom = new Room("MonsterRoom", "A dark cave. You hear growling.", blockExitIfMonsterAlive: true);

            // Exits
            start.AddExit(Direction.North, winRoom);
            start.AddExit(Direction.East, keyRoom);
            start.AddExit(Direction.South, swordRoom);
            start.AddExit(Direction.West, deathRoom);

            keyRoom.AddExit(Direction.West, start);

            swordRoom.AddExit(Direction.North, start);
            swordRoom.AddExit(Direction.South, monsterRoom);

            monsterRoom.AddExit(Direction.North, swordRoom);

            // Items
            keyRoom.Items.Add(new Item("Key", "A rusty key.", ItemType.Key));
            swordRoom.Items.Add(new Item("Sword", "A sharp blade.", ItemType.Weapon));

            // Monsters
            monsterRoom.Monster = new Monster("Dragon", 50, 20, requiresWeapon: true);

            // All rooms in a list
            List<Room> allRooms = new List<Room>
            {
                start, deathRoom, keyRoom, winRoom, swordRoom, monsterRoom
            };

            return new Rooms(allRooms, start);
        }
    }
}
