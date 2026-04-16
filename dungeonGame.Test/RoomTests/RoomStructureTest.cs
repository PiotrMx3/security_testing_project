using NUnit.Framework;
using DungeonGame; // Nu je de namespaces hebt aangepast, is dit genoeg!

namespace DungeonGame.Test.RoomTests
{
    [TestFixture]
    internal class RoomStructureTests
    {
        [Test]
        public void Constructor_ShouldAssignPropertiesCorrectly()
        {
            // 1. ARRANGE
            string name = "Kelder";
            string description = "Het is hier donker en vochtig.";
            bool isDeadly = true;

            // 2. ACT
            var room = new Room(name, description, isDeadly: isDeadly);

            // 3. ASSERT
            Assert.Multiple(() =>
            {
                Assert.That(room.Name, Is.EqualTo(name));
                Assert.That(room.Description, Is.EqualTo(description));
                Assert.That(room.IsDeadly, Is.True);
            });
        }

        [Test]
        public void ItemsList_ShouldBeInitialized_AsEmptyList()
        {
            // 1. ARRANGE & 2. ACT
            var room = new Room("Test", "Test");

            // 3. ASSERT
            Assert.That(room.Items, Is.Not.Null);
            Assert.That(room.Items, Is.Empty);
        }

        [Test]
        public void DefaultValues_ShouldBeCorrect()
        {
            // 1. ARRANGE & 2. ACT
            var room = new Room("Standaard", "Omschrijving");

            // 3. ASSERT
            Assert.Multiple(() =>
            {
                Assert.That(room.IsDeadly, Is.False, "Default IsDeadly should be false");
                Assert.That(room.IsLocked, Is.False, "Default IsLocked should be false");
                Assert.That(room.BlockExitIfMonsterAlive, Is.False, "Default BlockExit should be false");
                Assert.That(room.RequiredKeyName, Is.Null, "Default KeyName should be null");
            });
        }
    }
}