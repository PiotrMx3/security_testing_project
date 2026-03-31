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
        public void Adding_Item_Returns_That_Item()
        {
            // Arrange
            bool excpected = true;                
            // Act
            bool actual = _sut.Add(_sword);

            // Assert
            Assert.That(actual, Is.EqualTo(excpected));


        }

    }
}

