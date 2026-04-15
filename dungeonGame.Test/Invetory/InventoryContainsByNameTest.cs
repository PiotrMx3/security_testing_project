using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.Test.Invetory
{
    internal class InventoryContainsByNameTest
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
        public void ContainsByName_ExistingItem_ReturnsTrue()
        {
            //Arrange
            _sut.Add(_sword);
            //Act
            bool actual = _sut.Contains("Sword");
            //Assert
            Assert.That(actual, Is.True);
        }

        [Test]
        public void ContainsByName_DuplicateItemAfterRemovingOne_ReturnsTrue()
        {
            //Arrange
            _sut.Add(_sword);
            _sut.Add(_sword);
            _sut.Remove(_sword);
            //Act
            bool actual = _sut.Contains("Sword");
            //Assert
            Assert.That(actual, Is.True);
        }

        [Test]
        public void ContainsByName_EmptyCollection_ReturnsFalse()
        {
            //Arrange
            //Act
            bool actual = _sut.Contains("Sword");
            //Assert
            Assert.That(actual, Is.False);
        }

        [Test]
        public void ContainsByName_NotExistingItem_ReturnsFalse()
        {
            //Arrange
            _sut.Add(_sword);
            //Act
            bool actual = _sut.Contains("Dagger");
            //Assert
            Assert.That(actual, Is.False);
        }

        [Test]
        public void ContainsByName_WrongCasing_ReturnsFalse()
        {
            //Arrange
            _sut.Add(_sword);
            //Act
            bool actual = _sut.Contains("sword");
            //Assert
            Assert.That(actual, Is.False);
        }
    }
}
