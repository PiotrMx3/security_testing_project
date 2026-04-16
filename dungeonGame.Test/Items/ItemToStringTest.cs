using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.Test.Items
{
    [TestFixture]
    internal class ItemToStringTest
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

        }
    }
}
