namespace DungeonGame
{
    public enum ItemType
    {
        Weapon,
        Key,
        Consumable,
        Note
    }

    /// <summary>
    /// Een concreet object in de game, zoals een wapen of een sleutel.
    /// </summary>
    public class Item : IItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ItemType Type { get; set; }

        /// <summary>
        /// Initialiseert een nieuwe instantie van de <see cref="Item"/> klasse.
        /// </summary>
        /// <param name="name">De naam van het item.</param>
        /// <param name="description">De omschrijving van het item.</param>
        /// <param name="type">Het type item (Weapon, Key, Consumable).</param>
        public Item(string name, string description, ItemType type)
        {
            Name = name;
            Description = description;
            Type = type;
        }

        /// <summary>
        /// Geeft de details van het item terug als een leesbare string.
        /// </summary>
        /// <returns>Een string in het formaat: "Naam (Type): Beschrijving".</returns>
        public override string ToString()
        {
            return $"{Name} ({Type}): {Description}";
        }
    }
}
