
using DungeonGame.Models;

namespace DungeonGame
{
    public interface IInventory
    {
        List<IItem> Items { get; }
        int MaxCapacity { get; set; }

        bool Add(IItem item);
        bool Contains(IItem item);
        bool Contains(string name);
        bool HasKey(string keyName);
        bool HasWeapon();
        bool Remove(IItem item);
    }
}