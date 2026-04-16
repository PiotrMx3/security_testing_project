
using DungeonGame.Models;

namespace DungeonGame
{
    public interface IRooms
    {
        List<IRoom> AllRooms { get; set; }
        IRoom CurrentRoom { get; set; }

        bool Fight(IPlayer player);
        bool Move(Direction direction, IPlayer player);
    }
}