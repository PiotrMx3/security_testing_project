using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame
{
    public interface IRoom
    {
        string Name { get; set; }
        string Description { get; set; }
        Dictionary<Direction, IRoom> Exits { get; set; }
        List<IItem> Items { get; set; }
        IMonster? Monster { get; set; }
        bool IsDeadly { get; set; }
        bool IsLocked { get; set; }
        string? RequiredKeyName { get; set; }
        bool BlockExitIfMonsterAlive { get; set; }
        void AddExit(Direction direction, IRoom room);
        bool HasExit(Direction direction);
        bool CanEnter(IInventory inventory);
        IItem? TakeItem(string itemName);
        string Describe();
    }
}
