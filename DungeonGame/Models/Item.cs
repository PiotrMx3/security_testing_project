using DungeonGame.Models;

namespace DungeonGame
{
    public enum ItemType
    {
        Weapon,
        Key,
        Consumable
    }

    public class Item : IItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ItemType Type { get; set; }

        public Item(string name, string description, ItemType type)
        {
            Name = name;
            Description = description;
            Type = type;
        }

        public override string ToString()
        {
            return $"{Name} ({Type}): {Description}";
        }
    }
}
