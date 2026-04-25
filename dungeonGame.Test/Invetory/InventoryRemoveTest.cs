using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DungeonGame.Test.Invetory
{
    internal class InventoryRemoveTest
    {
        private Inventory _sut;
        private Item _sword;
        private Item _dagger;

        [SetUp]
        public void Setup()
        {
            this._sut = new Inventory(maxCapacity: 5);
            this._sword = new Item("Sword", "A sharp blade.", ItemType.Weapon);
            this._dagger = new Item("Dagger", "A strong dagger.", ItemType.Weapon);
        }


        [Test]
        public void Remove_ItemFromEmptyCollection_ReturnsFalse()
        {
            //Arrange

            //Act
            bool actual = _sut.Remove(_sword);

            //Assert
            Assert.That(actual, Is.False);

        }

        [Test]
        public void Remove_ExistingItem_ReturnsTrue()
        {
            //Arrange
            _sut.Add(_sword);

            //Act
            bool actual = _sut.Remove(_sword);

            //Assert
            Assert.That(actual, Is.True);

        }


        [Test]
        public void Remove_NotExistingItem_ReturnsFalse()
        {
            //Arrange
            _sut.Add(_sword);
            _sut.Add(_dagger);

            Item test = new("Test", "Test",ItemType.Weapon);

            //Act
            bool actual = _sut.Remove(test);

            //Assert
            Assert.That(actual, Is.False);

        }

    }
}
