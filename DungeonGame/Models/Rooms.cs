using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame
{
    public class Rooms
    {
        public List<Room> AllRooms { get; set; }
        public Room CurrentRoom { get; set; }

        public Rooms(List<Room> allRooms, Room startRoom)
        {
            AllRooms = allRooms;
            CurrentRoom = startRoom;
        }

        public bool Move(Direction direction, Player player)
        {
            // Check: monster blocks exit (leave monster room while alive = death)
            if (CurrentRoom.BlockExitIfMonsterAlive
                && CurrentRoom.Monster != null
                && CurrentRoom.Monster.IsAlive)
            {
                player.Health = 0;
                return false;
            }

            // Check: exit exists
            if (!CurrentRoom.HasExit(direction))
                return false;

            Room nextRoom = CurrentRoom.Exits[direction];

            // Check: room is locked and player has no key
            if (!nextRoom.CanEnter(player.Inventory))
                return false;

            // Move player to new room
            CurrentRoom = nextRoom;

            // Check: deadly room = instant death
            if (CurrentRoom.IsDeadly)
            {
                player.Health = 0;
                return true;
            }

            return true;
        }

        public bool Fight(Player player)
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
                bool rng = new Random().Next(0, 100) < 25;
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
