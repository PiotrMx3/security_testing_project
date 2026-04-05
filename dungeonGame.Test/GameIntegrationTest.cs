using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.Test
{
    [TestFixture]
    public class GameIntegrationTest
    {
        private Rooms rooms;
        private Room startRoom;
        private Room keyRoom;
        private Room swordRoom;
        private Room monsterRoom;
        private Room deathRoom;
        private Room winRoom;

        [SetUp]
        public void Setup()
        {
            // ---------------------------
            // Rooms
            // ---------------------------
            startRoom = new Room("Start", "Startkamer");
            keyRoom = new Room("KeyRoom", "Hier ligt een sleutel.");
            swordRoom = new Room("SwordRoom", "Hier ligt een zwaard.");
            monsterRoom = new Room("MonsterRoom", "Een monster bewaakt deze kamer.", blockExitIfMonsterAlive: true);
            deathRoom = new Room("DeathRoom", "Deze kamer is dodelijk!", isDeadly: true);
            winRoom = new Room("WinRoom", "De uitgang is hier.", isLocked: true, requiredKeyName: "Key");

            // Exits
            startRoom.AddExit(Direction.North, winRoom);
            startRoom.AddExit(Direction.East, keyRoom);
            startRoom.AddExit(Direction.South, swordRoom);
            startRoom.AddExit(Direction.West, deathRoom);

            swordRoom.AddExit(Direction.North, startRoom);
            swordRoom.AddExit(Direction.South, monsterRoom);
            monsterRoom.AddExit(Direction.North, swordRoom);
            keyRoom.AddExit(Direction.West, startRoom);

            // ---------------------------
            // Items
            // ---------------------------
            keyRoom.Items.Add(new Item("Key", "Een roestige sleutel.", ItemType.Key));
            swordRoom.Items.Add(new Item("Sword", "Een scherp zwaard.", ItemType.Weapon));

            // ---------------------------
            // Monster
            // ---------------------------
            monsterRoom.Monster = new Monster("Dragon", 30, 10, requiresWeapon: true);

            // ---------------------------
            // Rooms object
            // ---------------------------
            var allRooms = new List<Room> { startRoom, keyRoom, swordRoom, monsterRoom, deathRoom, winRoom };
            rooms = new Rooms(allRooms, startRoom);
        }

        // ---------------------------
        // Full game integration test
        // ---------------------------
        [Test]
        public void FullGameFlow_PlayerWins()
        {
            var player = new Player("Hero", 100);
            var game = new Game(player, rooms);

            // 1. Move to key room and take key
            bool moved = game.Move("e");
            Console.WriteLine($"Move to keyRoom: {moved}, Current room: {game.Rooms.CurrentRoom.Name}");
            bool taken = game.Take("Key");
            Console.WriteLine($"Take Key: {taken}");
            Console.WriteLine($"Has Key: {player.Inventory.HasKey("Key")}");

            // 2. Move back to start
            moved = game.Move("w");
            Console.WriteLine($"Move back to start: {moved}, Current room: {game.Rooms.CurrentRoom.Name}");

            // 3. Move to sword room and take sword
            moved = game.Move("s");
            Console.WriteLine($"Move to swordRoom: {moved}, Current room: {game.Rooms.CurrentRoom.Name}");
            taken = game.Take("Sword");
            Console.WriteLine($"Take Sword: {taken}");
            Console.WriteLine($"Has Weapon: {player.Inventory.HasWeapon()}");

            // 4. Move to monster room
            moved = game.Move("s");
            Console.WriteLine($"Move to monsterRoom: {moved}, Current room: {game.Rooms.CurrentRoom.Name}");

            // 5. Fight monster
            bool fightResult = game.Fight();
            Console.WriteLine($"Fight result: {fightResult}");
            Console.WriteLine($"Monster alive: {game.Rooms.CurrentRoom.Monster?.IsAlive}");
            Console.WriteLine($"Player alive: {player.IsAlive}");

            // 6. Return to start
            Assert.IsTrue(game.Move("n"), "Failed to move north from monsterRoom to swordRoom");
            Console.WriteLine($"Moved to: {game.Rooms.CurrentRoom.Name}");
            Assert.IsTrue(game.Move("n"), "Failed to move north from swordRoom to startRoom");
            Console.WriteLine($"Moved to: {game.Rooms.CurrentRoom.Name}");

            // 7. Unlock and move to win room
            Console.WriteLine($"Can enter winRoom? {winRoom.CanEnter(player.Inventory)}");
            Console.WriteLine($"winRoom.IsLocked: {winRoom.IsLocked}");
            Console.WriteLine($"Has Key: {player.Inventory.HasKey("Key")}");

            Assert.IsTrue(game.Move("n"), "Failed to move north from startRoom to winRoom");
            Console.WriteLine($"Moved to: {game.Rooms.CurrentRoom.Name}");

            // 8. Check win
            Assert.IsTrue(game.CheckWin());
            Assert.IsTrue(player.IsWinner);

            // 9. Player still alive
            Assert.IsTrue(player.IsAlive);
            Assert.Greater(player.Health, 0);
        }

        [Test]
        public void FullGameFlow_PlayerDiesInDeadlyRoom()
        {
            var player = new Player("Hero", 100);
            var game = new Game(player, rooms);

            // Move to deadly room
            Assert.IsTrue(game.Move("w")); // west = deathRoom
            Assert.IsFalse(player.IsAlive);
            Assert.AreEqual(0, player.Health);
        }
    }
}
