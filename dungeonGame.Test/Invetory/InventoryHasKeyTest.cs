using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.Test.Invetory
{
    internal class InventoryHasKeyTest
    {
        private Inventory _sut;
        private Item _sword;
        private Item _key;
        private Item _secretKey;

        [SetUp]
        public void Setup()
        {
            this._sut = new Inventory(maxCapacity: 5);
            this._sword = new Item("Sword", "A sharp blade.", ItemType.Weapon);
            this._key = new Item("DungeonKey", "Key to the dungeon.", ItemType.Key);
            this._secretKey = new Item("SecretKey", "Key to unlock a secret.", ItemType.Key);
        }

        [Test]
        public void HasKey_ExistingKeyByName_ReturnsTrue()
        {
            //Arrange
            _sut.Add(_key);
            //Act
            bool actual = _sut.HasKey("DungeonKey");
            //Assert
            Assert.That(actual, Is.True);
        }

        [Test]
        public void HasKey_KeyExistsButDifferentName_ReturnsFalse()
        {
            //Arrange
            _sut.Add(_key);
            //Act
            bool actual = _sut.HasKey("SecretKey");
            //Assert
            Assert.That(actual, Is.False);
        }

        [Test]
        public void HasKey_ItemWithSameNameButNotKeyType_ReturnsFalse()
        {
            //Arrange
            var weaponNamedKey = new Item("DungeonKey", "A weapon.", ItemType.Weapon);
            _sut.Add(weaponNamedKey);
            //Act
            bool actual = _sut.HasKey("DungeonKey");
            //Assert
            Assert.That(actual, Is.False);
        }

        [Test]
        public void HasKey_EmptyCollection_ReturnsFalse()
        {
            //Arrange
            //Act
            bool actual = _sut.HasKey("DungeonKey");
            //Assert
            Assert.That(actual, Is.False);
        }

        [Test]
        public void HasKey_WrongCasing_ReturnsFalse()
        {
            //Arrange
            _sut.Add(_key);
            //Act
            bool actual = _sut.HasKey("dungeonkey");
            //Assert
            Assert.That(actual, Is.False);
        }
    }
}
