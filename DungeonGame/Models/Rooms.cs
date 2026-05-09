using DungeonGame.Interfaces;
using DungeonGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame
{
    public class Rooms : IRooms
    {
        public List<IRoom> AllRooms { get; set; }
        public IRoom CurrentRoom { get; set; }
        public IRandom Rnd { get; set; }

        public Rooms(List<IRoom> allRooms, IRoom startRoom, IRandom rnd)
        {
            AllRooms = allRooms;
            CurrentRoom = startRoom;
            Rnd = rnd;
        }

        public Rooms(List<IRoom> allRooms, IRoom startRoom) : this (allRooms, startRoom, new RandomStub())
        {

        }

        /// <summary>
        /// Verwerkt de verplaatsing van de speler tussen kamers.
        /// </summary>
        /// <param name="direction">De gekozen richting.</param>
        /// <param name="player">De speler die zich verplaatst.</param>
        /// <param name="authService">De authenticatie-dienst voor rol-gebaseerde toegang (SEC-14).</param>
        /// <returns>True als de verplaatsing is gelukt.</returns>
        public bool Move(Direction direction, IPlayer player, IAuthService authService)
        {
            // SEC-14: Als een Admin 'noclip' heeft, negeert hij ook de blokkade door monsters?
            // In de meeste games betekent noclip dat je overal langs mag. 
            // Wil je dat de Admin ook veilig langs monsters kan? Dan voegen we dit toe:
            bool isAdmin = authService.UserRole == "Admin";

            // Check: monster blocks exit
            if (!isAdmin && CurrentRoom.BlockExitIfMonsterAlive
                && CurrentRoom.Monster != null
                && CurrentRoom.Monster.IsAlive)
            {
                player.Health = 0;
                return false;
            }

            // Check: exit exists
            if (!CurrentRoom.HasExit(direction))
                return false;

            // Hier wordt de volgende kamer bepaald uit de Dictionary van de huidige kamer
            IRoom nextRoom = CurrentRoom.Exits[direction];

            // SEC-14: Check of de kamer toegankelijk is. 
            // We geven nu de authService mee zodat Admins zonder sleutel naar binnen mogen.
            if (!nextRoom.CanEnter(player.Inventory, authService))
            {
                return false;
            }

            // Verplaats de speler
            CurrentRoom = nextRoom;

            // Check: deadly room = instant death (ook voor Admins, tenzij je noclip nog verder trekt)
            if (!isAdmin && CurrentRoom.IsDeadly)
            {
                player.Health = 0;
                return true;
            }

            return true;
        }

        public bool Fight(IPlayer player)
        {
            if (CurrentRoom.Monster == null || !CurrentRoom.Monster.IsAlive)
            {
                return false;
            }

            if (CurrentRoom.Monster.RequiresWeapon && !player.Inventory.HasWeapon())
            {
                player.Health = 0;
                return false;
            }

            while (CurrentRoom.Monster.IsAlive && player.IsAlive)
            {
                bool rng = Rnd.Next() < 25;
                int rDamage = rng ? 5 : 20;

                CurrentRoom.Monster.TakeDamage(rDamage);

                if (!CurrentRoom.Monster.IsAlive) return true;

                CurrentRoom.Monster.Attack(player);

                if (!player.IsAlive) return false;
            }

            return false;
        }
    }
}
