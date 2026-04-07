using Moq;

namespace DungeonGame.Test;

[TestFixture]
public class RoomsTests
{
    [Test]
    public void Move_Monster_alive_and_Block_exit_PlayerDies()
    {
        var player = new Player("TestPlayer", 100);
        var mockMonster = new Mock<IMonster>();

        mockMonster.Setup(m => m.IsAlive).Returns(true);

        var startRoom = new Room("Start", "Starting room", blockExitIfMonsterAlive: true);
        var nextRoom = new Room("Next", "Next room");

        startRoom.Monster = mockMonster.Object;
        startRoom.AddExit(Direction.North, nextRoom);

        var rooms = new Rooms(new List<Room> { startRoom, nextRoom }, startRoom);

        rooms.Move(Direction.North, player);

        Assert.AreEqual(0, player.Health);
        Assert.AreEqual(startRoom, rooms.CurrentRoom);
    }

}
