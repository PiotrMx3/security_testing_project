using DungeonGame.Interfaces;

namespace DungeonGame.Test;

[TestFixture]
public class MonsterRoomIntegrationTests
{
    [Test]
    public void Integration_PlayerCanLeaveMonsterRoomAfterKillingTheMonster()
    {
        //arrange
        var previousRoom = new Room("Previous room", "Safe room", false, false, null, false);
        var monsterRoom = new Room("Monster room", "testing", false, false, null,true); 
        var monster = new Monster("Orc", 1, 0, false);
        var player = new Player("Testhero", 100);
        monsterRoom.Monster = monster;
        monsterRoom.AddExit(Direction.South, previousRoom);
        previousRoom.AddExit(Direction.North,monsterRoom);
        var rooms = new Rooms(new List<IRoom> { monsterRoom, previousRoom }, monsterRoom);

        //act
        rooms.Fight(player);
        bool result = rooms.Move(Direction.South, player);

        //assert


        Assert.IsTrue(monsterRoom.Monster.IsAlive == false);
        Assert.IsTrue(result);
        Assert.AreEqual(previousRoom, rooms.CurrentRoom);

    }
}
