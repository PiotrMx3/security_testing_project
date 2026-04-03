namespace DungeonGame
{
    public class Player : IPlayer
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public bool IsAlive => Health > 0;
        public bool IsWinner { get; set; }
        public Inventory Inventory { get; set; }
        public Room CurrentRoom { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        IInventory IPlayer.Inventory { get => Inventory; set => throw new NotImplementedException(); }

        public Player(string name, int health)
        {
            Name = name;
            Health = health;
            IsWinner = false;
            Inventory = new Inventory();
        }

        public void TakeDamage(int amount)
        {
            if (amount < 0) return;
            Health -= amount;
            if (Health < 0) Health = 0;
        }

        public bool Move(string direction)
        {
            throw new NotImplementedException();
        }

        public bool PickUpItem(Item item)
        {
            throw new NotImplementedException();
        }
    }
}
