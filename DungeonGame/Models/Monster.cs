namespace DungeonGame
{


    public class Monster : IMonster
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public int AttackDamage { get; set; }
        public bool RequiresWeapon { get; set; }
        public bool IsAlive => Health > 0;

        public Monster(string name, int health, int attackDamage, bool requiresWeapon = false)
        {
            Name = name;
            Health = health;
            AttackDamage = attackDamage;
            RequiresWeapon = requiresWeapon;
        }

        public void TakeDamage(int amount)
        {
            if (amount < 0) return;
            Health -= amount;
            if (Health < 0) Health = 0;
        }

        public void Attack(IPlayer player)
        {
            player.TakeDamage(AttackDamage);
        }
    }

}
