using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.Test
{
    [TestFixture]
    internal class GameSetupTest
    {
        [Test]
        public void CreateWorld_ShouldSetupRoomsCorrectly()
        {
            // Act
            Rooms rooms = GameSetup.CreateWorld();

            // Assert start room
            Assert.That(rooms.CurrentRoom.Name, Is.EqualTo("Start"));

            // Assert aantal rooms
            Assert.That(rooms.AllRooms.Count, Is.EqualTo(6));

            // Zoek rooms
            var start = rooms.AllRooms.First(r => r.Name == "Start");
            var keyRoom = rooms.AllRooms.First(r => r.Name == "KeyRoom");
            var winRoom = rooms.AllRooms.First(r => r.Name == "WinRoom");
            var swordRoom = rooms.AllRooms.First(r => r.Name == "SwordRoom");
            var monsterRoom = rooms.AllRooms.First(r => r.Name == "MonsterRoom");
            var deathRoom = rooms.AllRooms.First(r => r.Name == "DeathRoom");

            // Exits check
            Assert.That(start.HasExit(Direction.North), Is.True);
            Assert.That(start.Exits[Direction.North], Is.EqualTo(winRoom));
            Assert.That(start.HasExit(Direction.East), Is.True);
            Assert.That(start.Exits[Direction.East], Is.EqualTo(keyRoom));
            Assert.That(start.HasExit(Direction.South), Is.True);
            Assert.That(start.Exits[Direction.South], Is.EqualTo(swordRoom));
            Assert.That(start.HasExit(Direction.West), Is.True);
            Assert.That(start.Exits[Direction.West], Is.EqualTo(deathRoom));

            // KeyRoom → moet terug naar start kunnen
            Assert.That(keyRoom.HasExit(Direction.West), Is.True);
            Assert.That(keyRoom.Exits[Direction.West], Is.EqualTo(start));

            // SwordRoom → moet naar monsterRoom kunnen
            Assert.That(swordRoom.HasExit(Direction.South), Is.True);
            Assert.That(swordRoom.Exits[Direction.South], Is.EqualTo(monsterRoom));

            // MonsterRoom → moet terug kunnen
            Assert.That(monsterRoom.HasExit(Direction.North), Is.True);
            Assert.That(monsterRoom.Exits[Direction.North], Is.EqualTo(swordRoom));

            // Items check
            Assert.That(keyRoom.Items.Any(i => i.Name == "Key"), Is.True);
            Assert.That(swordRoom.Items.Any(i => i.Name == "Sword"), Is.True);

            // Monster check
            Assert.That(monsterRoom.Monster, Is.Not.Null);
            Assert.That(monsterRoom.Monster.Name, Is.EqualTo("Dragon"));

            // lock check
            Assert.That(winRoom.IsLocked, Is.True);
            Assert.That(winRoom.RequiredKeyName, Is.EqualTo("Key"));
        }
    }
}
