namespace DungeonGame
{
    public class Player
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public bool IsAlive => Health > 0;
        public bool IsWinner { get; set; }
        public Inventory Inventory { get; set; }

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
    }
}
