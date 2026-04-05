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
            bool taken = game.Take("Key");

            // 2. Move back to start
            moved = game.Move("w");

            // 3. Move to sword room and take sword
            moved = game.Move("s");
            taken = game.Take("Sword");

            // 4. Move to monster room
            moved = game.Move("s");

            // 5. Fight monster
            bool fightResult = game.Fight();

            // 6. Return to start
            Assert.IsTrue(game.Move("n")); // back to swordRoom
            Assert.IsTrue(game.Move("n")); // back to startRoom

            // 7. Unlock and move to win room
            Assert.IsFalse(winRoom.IsLocked); // should auto-unlock als speler de key heeft
            Assert.IsTrue(game.Move("n")); // naar winRoom

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
