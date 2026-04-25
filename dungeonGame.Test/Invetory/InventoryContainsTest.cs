using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.Test.Invetory
{
    internal class InventoryContainsTest
    {
        private Inventory _sut;
        private Item _sword;

        [SetUp]
        public void Setup()
        {
            this._sut = new Inventory(maxCapacity: 5);
            this._sword = new Item("Sword", "A sharp blade.", ItemType.Weapon);
        }

        [Test]
        public void Contains_ExistingItem_ReturnsTrue()
        {
            //Arrange
            _sut.Add(_sword);

            //Act
            bool actual = _sut.Contains(_sword);

            //Assert
            Assert.That(actual, Is.True);
        }


        [Test]
        public void Contains_NotExistingItem_ReturnsFalse()
        {
            //Arrange
            _sut.Add(_sword);
            _sut.Add(_sword);

            Item test = new("Test", "Test", ItemType.Weapon);

            //Act
            bool actual = _sut.Contains(test);

            //Assert
            Assert.That(actual, Is.False);
        }

    }
}
