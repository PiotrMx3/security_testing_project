using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.Test.Invetory
{
    internal class InventoryHasWeaponTest
    {
        private Inventory _sut;
        private Item _sword;
        private Item _dagger;
        private Item _key;
        private Item _recoveryKit;



        [SetUp]
        public void Setup()
        {
            this._sut = new Inventory(maxCapacity: 5);
            this._sword = new Item("Sword", "A sharp blade.", ItemType.Weapon);
            this._dagger = new Item("Dagger", "A strong dagger.", ItemType.Weapon);
            this._key = new Item("Key", "Key to unlock an secret.", ItemType.Key);
            this._recoveryKit = new Item("RecoveryKit", "Add 25hp points", ItemType.Consumable);
        }



        [Test]
        public void HasWeapon_CollectionDonotContainsWeaponItem_ReturnsFalse()
        {
            //Arrange
            _sut.Add(_key);
            _sut.Add(_recoveryKit);

            //Act
            bool actual = _sut.HasWeapon();

            //Assert
            Assert.That(actual, Is.False);
        }

        [Test]
        public void HasWeapon_CollectionContainsWeaponItem_ReturnsTrue()
        {
            //Arrange
            _sut.Add(_dagger);

            //Act
            bool actual = _sut.HasWeapon();

            //Assert
            Assert.That(actual, Is.True);
        }

    }
}
