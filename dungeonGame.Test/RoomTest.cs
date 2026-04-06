using DungeonGame.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.Test
{
    public class RoomTest
    {
        private IRoom _room;
        private Mock<IInventory> _mockInventory;

        [SetUp]
        public void Setup()
        {
            // 1. ARRANGE
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
        // Describe Test
        [Test]
        public void Describe_ReturnsString()
        {
            // 1. ARRANGE (Word gedaan in SetUp)
            // Let op de @ voor de string (verbatim string), dit maakt het makkelijker met enters.
            string expected = "\n=== Boven ===\n" +
                              "Een kamer met een deur\n" +
                              "\nExits:\n";
            // 2. ACT
            string result = _room.Describe();
            // 3. ASSERT
            Assert.That(result, Is.EqualTo(expected));
        }
        // CanEnter Tests
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
        //
    }
}
