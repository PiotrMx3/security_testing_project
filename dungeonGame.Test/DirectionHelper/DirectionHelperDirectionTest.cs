using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame.Test.DirectionHelper
{
    public class DirectionHelperDirectionTest
    {
        [TestCase("n")]
        [TestCase("N")]
        [TestCase("north")]
        [TestCase("NORTH")]
        [TestCase("North")]
        public void Parse_NorthVariants_ReturnsNorth(string input)
        {
            // Act
            Direction? actual = DungeonGame.DirectionHelper.Parse(input);

            // Assert
            Assert.That(actual, Is.EqualTo(Direction.North));
        }

        [TestCase("e")]
        [TestCase("E")]
        [TestCase("east")]
        [TestCase("EAST")]
        [TestCase("East")]
        public void Parse_EastVariants_ReturnsEast(string input)
        {
            // Act
            Direction? actual = DungeonGame.DirectionHelper.Parse(input);

            // Assert
            Assert.That(actual, Is.EqualTo(Direction.East));
        }

        [TestCase("s")]
        [TestCase("S")]
        [TestCase("south")]
        [TestCase("SOUTH")]
        [TestCase("South")]
        public void Parse_SouthVariants_ReturnsSouth(string input)
        {
            // Act
            Direction? actual = DungeonGame.DirectionHelper.Parse(input);

            // Assert
            Assert.That(actual, Is.EqualTo(Direction.South));
        }

        [TestCase("w")]
        [TestCase("W")]
        [TestCase("west")]
        [TestCase("WEST")]
        [TestCase("West")]
        public void Parse_WestVariants_ReturnsWest(string input)
        {
            // Act
            Direction? actual = DungeonGame.DirectionHelper.Parse(input);

            // Assert
            Assert.That(actual, Is.EqualTo(Direction.West));
        }

        [TestCase("x")]
        [TestCase("up")]
        [TestCase("down")]
        [TestCase("")]
        [TestCase("norht")]
        public void Parse_InvalidInput_ReturnsNull(string input)
        {
            // Act
            Direction? actual = DungeonGame.DirectionHelper.Parse(input);

            // Assert
            Assert.That(actual, Is.Null);
        }
    }
}
