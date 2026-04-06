using DungeonGame.Models;

namespace DungeonGame
{

    public class Inventory : IInventory
    {
        public List<IItem> Items { get; private set; }
        public int MaxCapacity { get; set; }

        public Inventory(int maxCapacity = 5)
        {
            Items = new List<IItem>();
            MaxCapacity = maxCapacity;
        }

        public bool Add(IItem item)
        {
            if (Items.Count >= MaxCapacity)
                return false;

            Items.Add(item);
            return true;
        }

        public bool Remove(IItem item)
        {
            return Items.Remove(item);
        }

        public bool Contains(IItem item)
        {
            return Items.Contains(item);
        }

        public bool Contains(string name)
        {
            return Items.Any(i => i.Name == name);
        }

        public bool HasWeapon()
        {
            return Items.Any(i => i.Type == ItemType.Weapon);
        }

        public bool HasKey(string keyName)
        {
            return Items.Any(i => i.Type == ItemType.Key && i.Name == keyName);
        }
    }


}
