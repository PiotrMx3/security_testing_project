namespace DungeonGame
{
    public interface IMonster
    {
        int AttackDamage { get; set; }
        int Health { get; set; }
        bool IsAlive { get; }
        string Name { get; set; }
        bool RequiresWeapon { get; set; }

        void Attack(IPlayer player);
        void TakeDamage(int amount);
    }
}