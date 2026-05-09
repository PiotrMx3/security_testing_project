using System;
using System.Collections.Generic;
using System.Linq;

namespace DungeonGame
{
    /// <summary>
    /// De concrete implementatie van een rugzak (inventory).
    /// Beheert het toevoegen, verwijderen en zoeken van items binnen de capaciteitslimiet.
    /// </summary>
    public class Inventory : IInventory
    {

        public List<IItem> Items { get; } = new List<IItem>();
        public int MaxCapacity { get; set; }

        /// <summary>
        /// Initialiseert een nieuwe instantie van de <see cref="Inventory"/> klasse.
        /// </summary>
        /// <param name="maxCapacity">De maximale opslagcapaciteit (standaard 10).</param>
        public Inventory(int maxCapacity = 10)
        {
            MaxCapacity = maxCapacity;
        }

        /// <summary>
        /// Probeert een item toe te voegen aan de inventory, mits er nog ruimte is.
        /// </summary>
        /// <param name="item">Het <see cref="IItem"/> dat toegevoegd moet worden.</param>
        /// <returns>True als het item succesvol is toegevoegd; False als de limiet is bereikt.</returns>
        public bool Add(IItem item)
        {
            if (Items.Count >= MaxCapacity) return false;
            Items.Add(item);
            return true;
        }

        /// <summary>
        /// Controleert of een specifiek item-object fysiek aanwezig is in de lijst.
        /// </summary>
        /// <param name="item">Het te controleren item-object.</param>
        /// <returns>True als het object in de lijst staat; anders false.</returns>
        public bool Contains(IItem item) => Items.Contains(item);

        /// <summary>
        /// Controleert of er een item met een specifieke naam aanwezig is.
        /// </summary>
        /// <param name="name">De naam van het item waarnaar gezocht wordt.</param>
        /// <returns>True als een item met deze naam is gevonden (hoofdletterongevoelig).</returns>
        /// <remarks>Maakt gebruik van SEC-15 principes voor robuuste string-vergelijking.</remarks>
        public bool Contains(string name) =>
            Items.Any(i => i.Name.Equals(name?.Trim(), StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Controleert specifiek of de speler een bepaalde sleutel bezit. 
        /// Dit is essentieel voor de logica in <see cref="IRoom.CanEnter"/>.
        /// </summary>
        /// <param name="keyName">De naam van de benodigde sleutel.</param>
        /// <returns>True als een item van het type 'Key' met de juiste naam aanwezig is.</returns>
        /// <remarks>Onderdeel van de toegangscontrole-keten (SEC-14/15).</remarks>
        public bool HasKey(string keyName) =>
            Items.Any(i => i.Type == ItemType.Key && i.Name.Equals(keyName?.Trim(), StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Controleert of de inventory minimaal één item bevat van het type 'Weapon'.
        /// </summary>
        /// <returns>True als er een wapen aanwezig is; anders false.</returns>
        public bool HasWeapon() => Items.Any(i => i.Type == ItemType.Weapon);

        /// <summary>
        /// Verwijdert een specifiek item-object uit de inventory.
        /// </summary>
        /// <param name="item">Het te verwijderen item-object.</param>
        /// <returns>True als het item succesvol is gevonden en verwijderd.</returns>
        public bool Remove(IItem item) => Items.Remove(item);
    }
}