
namespace DungeonGame
{
    public interface IInventory
    {
        List<Item> Items { get; }
        int MaxCapacity { get; set; }

        bool Add(Item item);
        bool Contains(Item item);
        bool Contains(string name);
        bool HasKey(string keyName);
        bool HasWeapon();
        bool Remove(Item item);
    }
}