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
            _sut = new Inventory(maxCapacity: 5);
            _sword = new Item("Sword", "A sharp blade.", ItemType.Weapon);
        }

        [Test]
        public void Add_ValidItem_ReturnsTrue()
        {
            // Act
                _sut.Add(_sword);
            // Assert
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


        [Test]
        public void Add_SameItemTwice_BothSucceed()
        {

            // Act
            _sut.Add(_sword);
            _sut.Add(_sword);


            // Assert
            Assert.That(_sut.Items.Count, Is.EqualTo(2));

        }

        [Test]
        public void Add_ItemCountEquals_MaxCapacity_ReturnsTrue()
        {

            // Arrange
            int maxCapacity = 1;
            var inventory = new Inventory() { MaxCapacity = maxCapacity };

            // Act
            inventory.Add(_sword);

            // Assert
            Assert.That(inventory.Items.Count, Is.EqualTo(maxCapacity));

        }

    }
}

