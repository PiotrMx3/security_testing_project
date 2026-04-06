using DungeonGame.Models;

namespace DungeonGame
{
    public class Room : IRoom
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<Direction, Room> Exits { get; set; }
        public List<IItem> Items { get; set; }
        public IMonster? Monster { get; set; }
        public bool IsDeadly { get; set; }
        public bool IsLocked { get; set; }
        public string? RequiredKeyName { get; set; }
        public bool BlockExitIfMonsterAlive { get; set; }

        public Room(string name, string description, bool isDeadly = false,
                     bool isLocked = false, string? requiredKeyName = null,
                     bool blockExitIfMonsterAlive = false)
        {
            Name = name;
            Description = description;
            Exits = new Dictionary<Direction, Room>();
            Items = new List<IItem>();
            Monster = null;
            IsDeadly = isDeadly;
            IsLocked = isLocked;
            RequiredKeyName = requiredKeyName;
            BlockExitIfMonsterAlive = blockExitIfMonsterAlive;
        }

        public void AddExit(Direction direction, Room room)
        {
            Exits[direction] = room;
        }

        public bool HasExit(Direction direction)
        {
            return Exits.ContainsKey(direction);
        }

        public bool CanEnter(IInventory inventory)
        {
            if (!IsLocked) return true;
            if (RequiredKeyName == null) return false;
            return inventory.HasKey(RequiredKeyName);
        }

        public IItem? TakeItem(string itemName)
        {
            IItem? item = Items.FirstOrDefault(i => i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));
            if (item != null)
            {
                Items.Remove(item);
            }
            return item;
        }

        public string Describe()
        {
            string info = "";

            info += $"\n=== {Name} ===\n";
            info += $"{Description}\n";

            if (Items.Any())
            {
                info += "\nItems in this room:\n";
                foreach (IItem item in Items)
                {
                    info += $"  - {item}\n";
                }
            }

            if (Monster != null && Monster.IsAlive)
            {
                info += $"\nMonster: {Monster.Name} (HP: {Monster.Health})\n";
            }

            info += "\nExits:";
            foreach (var exit in Exits)
            {
                info += $" {exit.Key}";
            }
            info += "\n";

            return info;
        }
    }
}
