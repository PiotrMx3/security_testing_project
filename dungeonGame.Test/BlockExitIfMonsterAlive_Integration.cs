using DungeonGame.Interfaces;

namespace DungeonGame.Test;

[TestFixture]
public class BlockExitIfMonsterAlive_Integration
{
    [Test]
    public void Integration_PlayerDiesWhenFleeingRoomWithLivingMonster()
    {
        //arrange
        var player = new Player("Testhero", 100);
        var monster = new Monster("Orc", 100, 15, false);
        var monsterRoom = new Room("Monster Room", "A dark room", false, false, null, true);
        var previousRoom = new Room("Previous Room", "A safe room", false, false, null, false);

        monsterRoom.Monster = monster;
        monsterRoom.AddExit(Direction.South, previousRoom);
        previousRoom.AddExit(Direction.North, monsterRoom);
        var rooms = new Rooms(new List<IRoom> { monsterRoom, previousRoom }, monsterRoom);

        //act
        bool result = rooms.Move(Direction.South, player);
        //assert
        Assert.IsFalse(result);
        Assert.AreEqual(0, player.Health);
        Assert.IsFalse(player.IsAlive);
    }
}
