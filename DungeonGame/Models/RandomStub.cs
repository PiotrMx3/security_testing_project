using DungeonGame.Models.Interfaces;

namespace DungeonGame.Models
{
    public class RandomStub : IRandom
    {
        public Random _rnd = Random.Shared;
        public int Next()
        {
            return _rnd.Next(0, 100);
        }
    }
}
