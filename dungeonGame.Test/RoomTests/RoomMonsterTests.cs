using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.Test.RoomTests
{
    [TestFixture]
    internal class RoomMonsterTests
    {
        private Mock<IMonster> _mockedMonster;
        [SetUp]
        public void Setup()
        {
            // En een mocked monster
            _mockedMonster = new Mock<IMonster>();
        }
        [Test]
        public void Monster_ShouldBeNull_WhenRoomIsCreated()
        {
            // 1. ARANGE
            var room = new Room("Test", "Test");
            // 2. ACT - Niet nodig om Null te testen
            // 3. ASSERT
            Assert.That( room.Monster, Is.Null );
        }
        [Test]
        public void Monster_ShouldBeSettable()
        {
            // 1. ARANGE
            var room = new Room("Test", "Test");
            // 2. ACT
            room.Monster = _mockedMonster.Object;
            // 3. ASSERT
            Assert.That(room.Monster, Is.EqualTo(_mockedMonster.Object));
        }
        [Test]
        public void BlockExitIfMonsterIsAlive_ShouldDefaultToFalse()
        {
            // 1. ARANGE
            var room = new Room("Test", "Test");
            // 2. ACT - Niet Nodig
            // 3. ASSERT
            Assert.That(room.BlockExitIfMonsterAlive, Is.False );
        }
        [Test]
        public void BlockExitIfMonsterIsAlive_WhenSetInConstructor_ShouldStayTrue()
        {
            // 1. ARRANGE
            var room = new Room("Test", "Test", blockExitIfMonsterAlive: true);
            // 2. ACT - Niet nodig
            // 3. ASSERT
            Assert.That(room.BlockExitIfMonsterAlive, Is.True);
        }
    }
}
