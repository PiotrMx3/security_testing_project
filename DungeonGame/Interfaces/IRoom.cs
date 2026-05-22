using DungeonGame.Interfaces;
using DungeonGame.Services;

namespace DungeonGame
{
    /// <summary>
    /// Definieert een locatie in de spelwereld met navigatie en beveiligingseisen.
    /// </summary>
    public interface IRoom
    {
        string Name { get; set; }
        string Description { get; set; }
        Dictionary<Direction, IRoom> Exits { get; set; }
        List<IItem> Items { get; set; }
        IMonster? Monster { get; set; }
        bool IsDeadly { get; set; }
        bool IsLocked { get; set; }
        string? RequiredKeyName { get; set; }
        bool BlockExitIfMonsterAlive { get; set; }

        /// <summary>
        /// Maakt een verbinding aan tussen deze kamer en een andere kamer.
        /// </summary>
        /// <param name="direction">De windrichting van de uitgang.</param>
        /// <param name="room">De doelbestemming.</param>
        void AddExit(Direction direction, IRoom room);

        /// <summary>
        /// Controleert of er een doorgang bestaat in de opgegeven richting.
        /// </summary>
        /// <param name="direction">De te controleren richting.</param>
        /// <returns>True als er een kamer aan deze richting gekoppeld is.</returns>
        bool HasExit(Direction direction);

        /// <summary>
        /// Evalueert of een entiteit de kamer mag betreden op basis van bezit of rechten.
        /// </summary>
        /// <param name="inventory">De inventory van de speler voor sleutel-checks.</param>
        /// <param name="authService">De authenticatie-service voor rol-checks (SEC-14).</param>
        /// <returns>True als de toegang wordt verleend.</returns>
        bool CanEnter(IInventory inventory, IAuthService? authService);

        /// <summary>
        /// Probeert een item uit de kamer te halen op basis van de naam.
        /// </summary>
        /// <param name="itemName">De naam van het op te pakken item.</param>
        /// <returns>Het <see cref="IItem"/> object als het gevonden is, anders null.</returns>
        IItem? TakeItem(string itemName);

        /// <summary>
        /// Stelt een volledige beschrijving van de kamer samen voor de speler.
        /// </summary>
        /// <returns>Een string met de naam, omschrijving, items, monsters en uitgangen.</returns>
        string Describe();
        // encrypted room
        bool IsEncrypted { get; set; }
        string? EncryptionRoomId { get; set; }
        string? EncryptedFilePath { get; set; }
    }
}