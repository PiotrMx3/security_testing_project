using DungeonGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace DungeonGame.Test.RoomTests
{
    [TestFixture]
    internal class RoomTakeItemTest
    {
        private IRoom _room;
        private Mock<IItem> _mockedItem;
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
            // We maken een mocked item
            _mockedItem = new Mock<IItem>();
        }
        [Test]
        public void TakeItem_WhenItemExists_RetursItemAndRemoveFromRoom()
        {
            // 1. ARRANGE
            _mockedItem.Setup(i => i.Name).Returns("Sleutel");
            _room.Items.Add(_mockedItem.Object);
            // 2. ACT
            var result = _room.TakeItem("Sleutel");
            // 3. ASSERT
            Assert.That(result, Is.EqualTo(_mockedItem.Object));
            Assert.That(_room.Items, Is.Empty);
        }
        [Test]
        public void TakeItem_WhenItemDoesNotExists_ReturnsNull()
        {
            // 1. ARRANGE - We moeten geen items toevoegen om dit te testen
            // 2. ACT
            var result = _room.TakeItem("Sleutel");
            // 3. ASSERT
            Assert.That(result, Is.Null);
        }
        [Test]
        public void TakeItem_SearchIsCaseInsensitive_ReturnsItemAndRemoveFromRoom()
        {
            // 1. ARRANGE - Item = UpperCase
            _mockedItem.Setup(i => i.Name).Returns("Sleutel");
            _room.Items.Add( _mockedItem.Object);
            // 2. ACT - Item = lowerCase
            var result = _room.TakeItem("sleutel");
            // 3. ASSERT
            Assert.That(result, Is.EqualTo(_mockedItem.Object));
            Assert.That(_room.Items, Is.Empty);
        }
    }
}
/*
public IItem? TakeItem(string itemName)
{
    IItem? item = Items.FirstOrDefault(i => i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));
    if (item != null)
    {
        Items.Remove(item);
    }
    return item;
}
*/