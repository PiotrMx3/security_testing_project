using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.Test
{
    [TestFixture]
    internal class ItemTest
    {
        [Test]
        public void ToString_WhenCalled_ReturnsFormattedString()
        {
            // 1. ARRANGE
            var item = new Item("Sleutel", "Een gouden sleutel", ItemType.Key);
            // 2. ACT
            string result = item.ToString();
            // 3. ASSERT
            // De tekst moet exact zijn: $"{Name} ({Type}): {Description}";
            Assert.That(result, Is.EqualTo("Sleutel (Key): Een gouden sleutel"));
        }
    }
}
