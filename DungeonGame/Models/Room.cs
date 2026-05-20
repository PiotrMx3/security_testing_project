using DungeonGame.Interfaces;
using DungeonGame.Services;
using System.Text;

namespace DungeonGame
{
    /// <summary>
    /// Representeert een fysieke locatie in de dungeon. 
    /// Bevat de logica voor navigatie, items, monsters en toegangsbeveiliging.
    /// </summary>
    public class Room : IRoom
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Dictionary<Direction, IRoom> Exits { get; set; }

        public List<IItem> Items { get; set; }

        public IMonster? Monster { get; set; }

        public bool IsDeadly { get; set; }

        public bool IsLocked { get; set; }

        public string? RequiredKeyName { get; set; }

        public bool BlockExitIfMonsterAlive { get; set; }
        public bool IsEncrypted { get; set; }
        public string? EncryptionRoomId { get; set; }

        /// <summary>
        /// Initialiseert een nieuwe instantie van de <see cref="Room"/> klasse.
        /// </summary>
        /// <param name="name">De naam van de kamer.</param>
        /// <param name="description">Korte omschrijving van de sfeer of inhoud.</param>
        /// <param name="isDeadly">Indien true, sterft de speler bij binnenkomst.</param>
        /// <param name="isLocked">Bepaalt of de kamer vergrendeld is.</param>
        /// <param name="requiredKeyName">De naam van de sleutel die nodig is voor toegang.</param>
        /// <param name="blockExitIfMonsterAlive">Indien true, kan men de kamer niet verlaten zolang het monster leeft.</param>
        public Room(string name, string description, bool isDeadly = false,
                     bool isLocked = false, string? requiredKeyName = null,
                     bool blockExitIfMonsterAlive = false)
        {
            Name = name;
            Description = description;
            Exits = new Dictionary<Direction, IRoom>();
            Items = new List<IItem>();
            IsDeadly = isDeadly;
            IsLocked = isLocked;
            RequiredKeyName = requiredKeyName;
            BlockExitIfMonsterAlive = blockExitIfMonsterAlive;
        }

        /// <summary>
        /// Voegt een uitgang toe aan de kamer in een specifieke richting.
        /// </summary>
        /// <param name="direction">De windrichting waarin de nieuwe kamer zich bevindt.</param>
        /// <param name="room">De kamer waar de uitgang naartoe leidt.</param>
        public void AddExit(Direction direction, IRoom room) => Exits[direction] = room;

        /// <summary>
        /// Controleert of er een uitgang beschikbaar is in de opgegeven richting.
        /// </summary>
        /// <param name="direction">De te controleren windrichting.</param>
        /// <returns>True als er een uitgang bestaat; anders false.</returns>
        public bool HasExit(Direction direction) => Exits.ContainsKey(direction);

        /// <summary>
        /// Controleert of de speler (of admin) de kamer mag betreden.
        /// </summary>
        /// <param name="inventory">De inventory van de speler om op sleutels te controleren.</param>
        /// <param name="authService">De authenticatie-dienst om de gebruikersrol te verifiëren (SEC-14).</param>
        /// <returns>True als de toegang wordt verleend op basis van noclip of het bezit van de juiste sleutel.</returns>
        /// <remarks>Voldoet aan SEC-14 (Admin noclip) en SEC-15 (veilige afhandeling van data).</remarks>
        public bool CanEnter(IInventory inventory, IAuthService? authService)
        {
            // SEC-14: Admin noclip bypass op basis van rol in de JWT.
            if (authService?.UserRole == "Admin") return true;

            // Als de kamer niet op slot is, heeft iedereen toegang.
            if (!IsLocked) return true;

            // SEC-15: Veilige controle op configuratie om NullReferenceExceptions te voorkomen.
            if (string.IsNullOrEmpty(RequiredKeyName)) return false;

            // Controleer of de speler de benodigde sleutel in de inventory heeft.
            return inventory.HasKey(RequiredKeyName);
        }

        /// <summary>
        /// Zoekt een item in de kamer op naam en verwijdert het uit de kamer als het gevonden wordt.
        /// </summary>
        /// <param name="itemName">De naam van het item dat de speler probeert te pakken.</param>
        /// <returns>Het gevonden <see cref="IItem"/> object, of null als het item niet aanwezig is.</returns>
        /// <remarks>Maakt gebruik van SEC-15 principes zoals input-opschoning.</remarks>
        public IItem? TakeItem(string itemName)
        {
            // SEC-15: Input-opschoning (Trim) en robuuste, hoofdletterongevoelige vergelijking.
            IItem? item = Items.FirstOrDefault(i => i.Name.Equals(itemName?.Trim(), StringComparison.OrdinalIgnoreCase));

            if (item != null)
            {
                Items.Remove(item);
            }

            return item;
        }

        /// <summary>
        /// Stelt een volledige tekstuele beschrijving samen van de huidige status van de kamer.
        /// </summary>
        /// <returns>Een geformatteerde string met de naam, omschrijving, items, monsters en uitgangen.</returns>
        public string Describe()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"\n=== {Name} ===");
            sb.AppendLine(Description);

            // Toon de lijst met items als de kamer niet leeg is.
            if (Items.Any())
            {
                sb.AppendLine("\nItems in deze kamer:");
                foreach (IItem item in Items)
                {
                    sb.AppendLine($"  - {item}");
                }
            }

            // Toon informatie over het monster als er een levend monster aanwezig is.
            if (Monster != null && Monster.IsAlive)
            {
                sb.AppendLine($"\nPas op! Er staat een monster: {Monster.Name} (HP: {Monster.Health})");
            }

            // Toon een overzicht van de beschikbare windrichtingen voor navigatie.
            sb.Append("\nUitgangen:");
            foreach (var exit in Exits)
            {
                sb.Append($" {exit.Key}");
            }

            return sb.ToString();
        }
    }
}