using DungeonGame.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.Test.RoomTests
{
    [TestFixture]
    internal class RoomDescribeTests
    {
        private IRoom _room;

        [SetUp]
        public void Setup()
        {
            // We maken een kamer die op slot zit
            _room = new Room(
                "Boven",
                "Een kamer met een deur"
                );
        }
        [Test]
        public void Describe_ReturnsString()
        {
            // 1. ARRANGE (Word gedaan in SetUp)
            // Let op de @ voor de string (verbatim string), dit maakt het makkelijker met enters.
            string expected = $"{Environment.NewLine}=== Boven ==={Environment.NewLine}" +
                      $"Een kamer met een deur{Environment.NewLine}" +
                      $"{Environment.NewLine}Exits:{Environment.NewLine}";
            // 2. ACT
            string result = _room.Describe();
            // 3. ASSERT
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
