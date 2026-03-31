namespace DungeonGame
{
    public interface IPlayer
    {
        int Health { get; set; }
        IInventory Inventory { get; set; }
        bool IsAlive { get; }
        bool IsWinner { get; set; }
        string Name { get; set; }

        void TakeDamage(int amount);
    }
}