
namespace DungeonGame
{
    public interface IRooms
    {
        List<Room> AllRooms { get; set; }
        Room CurrentRoom { get; set; }

        bool Fight(IPlayer player);
        bool Move(Direction direction, IPlayer player);
    }
}