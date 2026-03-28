using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame
{
    public enum Direction
    {
        North,
        East,
        South,
        West
    }

    public static class DirectionHelper
    {
        public static Direction? Parse(string input)
        {
            string lower = input.ToLower();

            if (lower == "n" || lower == "north") return Direction.North;
            if (lower == "e" || lower == "east") return Direction.East;
            if (lower == "s" || lower == "south") return Direction.South;
            if (lower == "w" || lower == "west") return Direction.West;

            return null;
        }
    }
}
