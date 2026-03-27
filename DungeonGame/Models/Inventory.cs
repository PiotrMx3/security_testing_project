namespace DungeonGame
{

    public class Inventory
    {
        public List<Item> Items { get; private set; }
        public int MaxCapacity { get; set; }

        public Inventory(int maxCapacity = 5)
        {
            Items = new List<Item>();
            MaxCapacity = maxCapacity;
        }

        public bool Add(Item item)
        {
            if (Items.Count >= MaxCapacity)
                return false;

            Items.Add(item);
            return true;
        }

        public bool Remove(Item item)
        {
            return Items.Remove(item);
        }

        public bool Contains(Item item)
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
