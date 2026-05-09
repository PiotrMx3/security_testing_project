namespace DungeonGame
{
    /// <summary>
    /// Definieert de minimale eigenschappen van een object dat in de spelwereld kan bestaan.
    /// </summary>
    public interface IItem
    {
        /// <summary>De unieke naam van het item (bijv. "Gouden Sleutel").</summary>
        string Name { get; set; }

        /// <summary>Een smaakvolle beschrijving die de speler te zien krijgt bij inspectie.</summary>
        string Description { get; set; }

        /// <summary>Bepaalt het gedrag van het item (Wapen, Sleutel of Verbruiksartikel).</summary>
        ItemType Type { get; set; }

        /// <summary>Geeft een geformatteerde string terug voor weergave in de UI.</summary>
        string ToString();
    }
}