using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.Test.RoomTests
{
    [TestFixture]
    internal class RoomExitTests
    {
        private IRoom _room;
        [SetUp]
        public void Setup()
        {
            _room = new Room("Start", "De Begin Positie");
        }
        [Test]
        public void AddExit_ShouldStoreNeighborInExitsDictionary()
        {
            // 1. ARRANGE
            var neighborMock = new Mock<IRoom>();
            Direction targetDirection = Direction.North;
            // 2. ACT
            _room.AddExit(targetDirection, neighborMock.Object);
            // 3. ASSERT
            Assert.That(_room.Exits.ContainsKey(targetDirection), Is.True);
            Assert.That(_room.Exits[targetDirection], Is.EqualTo(neighborMock.Object));
        }
        [Test]
        public void HasExit_WhenDirectionIsAdded_ReturnsTrue()
        {
            // 1. ARRANGE
            var neighborMock = new Mock<IRoom>();
            _room.AddExit(Direction.North, neighborMock.Object);
            // 2. ACT - niet nodig
            // 3. ASSERT
            Assert.That(_room.HasExit(Direction.North), Is.True);
        }
        [Test]
        public void HasExit_WhenDirectionIsMissing_RetursFalse()
        {
            // 1. ARRANGE - Niet nodig
            // 2. ACT - Niet nodig
            // 3. ARRANGE
            Assert.That(_room.HasExit(Direction.South), Is.False);
        }
    }
}
