using DungeonGame.Interfaces;
using System.Collections.Generic;

namespace DungeonGame
{
    public class GameSetup
    {
        public static Rooms CreateWorld()
        {
            // 1. Maken van de kamers
            IRoom start = new Room("Start", "You are in a dark room. Exits lead in all directions.");
            IRoom deathRoom = new Room("DeathRoom", "The floor collapses beneath you!", isDeadly: true);
            IRoom keyRoom = new Room("KeyRoom", "A small dusty room. Something shines in the corner.");
            IRoom winRoom = new Room("WinRoom", "A bright room with a golden door. You made it!", isLocked: true, requiredKeyName: "Key");
            IRoom swordRoom = new Room("SwordRoom", "An old armory. Weapons hang on the walls.");
            IRoom monsterRoom = new Room("MonsterRoom", "A dark cave. You hear growling.", blockExitIfMonsterAlive: true);

            // 2. Koppelen van de deuren (Exits)
            start.AddExit(Direction.North, winRoom);
            start.AddExit(Direction.East, keyRoom);
            start.AddExit(Direction.South, swordRoom);
            start.AddExit(Direction.West, deathRoom);

            keyRoom.AddExit(Direction.West, start);
            swordRoom.AddExit(Direction.North, start);
            swordRoom.AddExit(Direction.South, monsterRoom);
            monsterRoom.AddExit(Direction.North, swordRoom);

            // 3. SEC-13: Beveiliging van Kamer 1 (WinRoom)
            winRoom.IsEncrypted = true;
            winRoom.EncryptionRoomId = "room1";
            winRoom.EncryptedFilePath = "EncryptedRooms/room_treasure.enc";

            // 4. SEC-13: Beveiliging van Kamer 2 (MonsterRoom)
            monsterRoom.IsEncrypted = true;
            monsterRoom.EncryptionRoomId = "room2";
            monsterRoom.EncryptedFilePath = "EncryptedRooms/room_monster.enc";

            // 5. Items plaatsen (Inclusief de wachtwoorden/passphrases!)
            start.Items.Add(new Item("note", "A dusty note saying: GeheimKamer1", ItemType.Note));
            keyRoom.Items.Add(new Item("Key", "A rusty key.", ItemType.Key));

            // Briefje voor de tweede versleutelde kamer verstoppen in de KeyRoom
            keyRoom.Items.Add(new Item("parchment", "An old parchment scratching: GeheimKamer2", ItemType.Note));
            swordRoom.Items.Add(new Item("Sword", "A sharp blade.", ItemType.Weapon));

            // Monsters
            monsterRoom.Monster = new Monster("Dragon", 50, 20, requiresWeapon: true);

            // Alle kamers registreren
            List<IRoom> allRooms = new List<IRoom>
            {
                start, deathRoom, keyRoom, winRoom, swordRoom, monsterRoom
            };

            return new Rooms(allRooms, start);
        }
    }
}