namespace DungeonGame
{
    public interface IPlayer
    {
        Room CurrentRoom { get; set; }
        int Health { get; set; }
        IInventory Inventory { get; set; }
        bool IsAlive { get; }
        bool IsWinner { get; set; }
        string Name { get; set; }

        bool Move(string direction);
        bool PickUpItem(Item item);
        void TakeDamage(int amount);
    }
}