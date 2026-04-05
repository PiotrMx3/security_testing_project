
namespace DungeonGame
{
    public interface IRoom
    {
        bool BlockExitIfMonsterAlive { get; set; }
        string Description { get; set; }
        Dictionary<Direction, Room> Exits { get; set; }
        bool IsDeadly { get; set; }
        bool IsLocked { get; set; }
        List<Item> Items { get; set; }
        Monster? Monster { get; set; }
        string Name { get; set; }
        string? RequiredKeyName { get; set; }

        void AddExit(Direction direction, Room room);
        bool CanEnter(IInventory inventory);
        string Describe();
        bool HasExit(Direction direction);
        Item? TakeItem(string itemName);
    }
}