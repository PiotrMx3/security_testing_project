using DungeonGame.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.Test.RoomTests
{
    [TestFixture]
    internal class RoomCanEnterTest
    {
        private IRoom _room;
        private Mock<IInventory> _mockInventory;

        [SetUp]
        public void Setup()
        {
            // We maken een kamer die op slot zit
            _room = new Room(
                "Boven",
                "Een kamer met een deur",
                isDeadly: false,
                isLocked: true,
                requiredKeyName: "Gouden sleutel",
                blockExitIfMonsterAlive: true
                );
            // We maken een mocked Inventory
            _mockInventory = new Mock<IInventory>();
        }
        [Test]
        public void CanEnter_LockedRoomWithCorrectKey_ReturnsTrue()
        {
            // 1. ARRANGE
            _mockInventory.Setup(i => i.HasKey("Gouden sleutel")).Returns(true);
            // 2. ACT
            bool result = _room.CanEnter(_mockInventory.Object);
            // 3. ASSERT
            Assert.That(result, Is.True);
        }
        [Test]
        public void CanEnter_LockedRoomWithoutKey_ReturnsFalse()
        {
            // 1. ARRANGE
            _mockInventory.Setup(i => i.HasKey("Gouden sleutel")).Returns(false);
            // 2. ACT
            bool result = _room.CanEnter(_mockInventory.Object);
            // 3. ASSERT
            Assert.That(result, Is.False);
        }
    }
}
