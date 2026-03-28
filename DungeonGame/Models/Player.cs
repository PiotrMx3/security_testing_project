namespace DungeonGame
{
    public class Player: IPlayer
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public bool IsAlive => Health > 0;
        public bool IsWinner { get; set; }
        public IInventory Inventory { get; set; }
        public Room CurrentRoom { get; set; }

        public Player(string name, int health, Room startingRoom)
        {
            Name = name;
            Health = health;
            IsWinner = false;
            Inventory = new Inventory();
            CurrentRoom = startingRoom;
        }

        public void TakeDamage(int amount)
        {
            if (amount < 0) return;
            Health -= amount;
            if (Health < 0) Health = 0;
        }

        public bool Move(string direction)
        {
            if (!CurrentRoom.TryGetExit(direction, out Room? nextRoom) || nextRoom == null)
                return false;

            if (!nextRoom.CanEnter(Inventory, CurrentRoom))
                return false;

            CurrentRoom = nextRoom;

            if (CurrentRoom.HasTrap)
                TakeDamage(CurrentRoom.TrapDamage);

            return true;
        }

        public bool PickUpItem(Item item)
        {
            if (!CurrentRoom.Items.Contains(item))
                return false;

            if (!Inventory.Add(item))
                return false;

            CurrentRoom.Items.Remove(item);

            return true;
        }
    }
}
