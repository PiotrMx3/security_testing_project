namespace DungeonGame
{
    /// <summary>
    /// Interface voor een opslagsysteem dat items beheert voor spelers of locaties.
    /// </summary>
    public interface IInventory
    {
        List<IItem> Items { get; }
        int MaxCapacity { get; set; }

        /// <summary>
        /// Probeert een item toe te voegen aan de verzameling.
        /// </summary>
        /// <param name="item">Het toe te voegen <see cref="IItem"/>.</param>
        /// <returns>True als het item is toegevoegd; False als de capaciteit is bereikt.</returns>
        bool Add(IItem item);

        /// <summary>
        /// Controleert of een specifiek item-object aanwezig is in de lijst.
        /// </summary>
        /// <param name="item">Het te zoeken item.</param>
        /// <returns>True als het exacte object gevonden is.</returns>
        bool Contains(IItem item);

        /// <summary>
        /// Controleert of er een item met een bepaalde naam aanwezig is.
        /// </summary>
        /// <param name="name">De naam van het item (bijv. "Key").</param>
        /// <returns>True als een item met deze naam bestaat.</returns>
        bool Contains(string name);

        /// <summary>
        /// Specifieke check voor toegangsbeveiliging.
        /// </summary>
        /// <param name="keyName">De naam van de benodigde sleutel.</param>
        /// <returns>True als er een item van het type Key met deze naam aanwezig is.</returns>
        bool HasKey(string keyName);

        /// <summary>
        /// Controleert of de speler een item van het type Weapon bezit.
        /// </summary>
        /// <returns>True als er minimaal één wapen aanwezig is.</returns>
        bool HasWeapon();

        /// <summary>
        /// Verwijdert een item uit de inventory.
        /// </summary>
        /// <param name="item">Het te verwijderen item.</param>
        /// <returns>True als het item succesvol is verwijderd.</returns>
        bool Remove(IItem item);
    }
}