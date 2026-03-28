namespace DungeonGame
{

    public class Room
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, Room> Exits { get; set; }
        public List<Item> Items { get; set; }
        public Monster? Monster { get; set; }
        public bool HasTrap { get; set; }
        public int TrapDamage { get; set; }
        public bool IsLocked { get; set; }
        public string? RequiredKeyName { get; set; }

        public Room(string name, string description, bool hasTrap = false, int trapDamage = 0, bool isLocked = false, string? requiredKeyName = null)
        {
            Name = name;
            Description = description;
            Exits = new Dictionary<string, Room>();
            Items = new List<Item>();
            Monster = null;
            HasTrap = hasTrap;
            TrapDamage = trapDamage;
            IsLocked = isLocked;
            RequiredKeyName = requiredKeyName;
        }

        public void AddExit(string direction, Room room)
        {
            Exits[direction] = room;
        }

        public bool TryGetExit(string direction, out Room? room)
        {
            return Exits.TryGetValue(direction, out room);
        }

        public bool CanEnter(Inventory inventory, Room currentRoom)
        {
            if (!IsLocked) return true;
            if (RequiredKeyName == null) return false;
            
            return inventory.HasKey(RequiredKeyName) && (currentRoom.Monster is null  || !currentRoom.Monster.IsAlive);
        }
    }



}
