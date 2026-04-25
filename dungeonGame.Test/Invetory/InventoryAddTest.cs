using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.Test.Invetory
{
    internal class InventoryAddTest
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
        public void Add_ValidItem_ReturnsTrue()
        {
            // Act
            bool actual =  _sut.Add(_sword);
            // Assert
            Assert.That(actual, Is.True);
            Assert.That(_sut.Items.Count, Is.EqualTo(1));

        }

        [Test]
        public void Add_MaxCapacity_ReturnsFalse()
        {
            // Arrange
            int maxCapacity = 1;
            var fullInventory = new Inventory() { MaxCapacity = maxCapacity };
            fullInventory.Add(_sword);

            // Act
            bool actual = fullInventory.Add(_sword);

            // Assert
            Assert.That(actual, Is.False);
            Assert.That(fullInventory.Items.Count, Is.EqualTo(maxCapacity));


        }


    }
}

