using Moq;

namespace DungeonGame.Test;

[TestFixture]
public class RoomsTests
{
    [Test]
    public void Move_Monster_alive_and_Block_exit_PlayerDies()
    {
        var mockPlayer = new Mock<IPlayer>();
        var mockInventory = new Mock<IInventory>();
        var mockMonster = new Mock<IMonster>();
        mockPlayer.Setup(p => p.Inventory).Returns(mockInventory.Object);
        mockPlayer.Setup(p => p.Health).Returns(100);
        mockMonster.Setup(m => m.IsAlive).Returns(true);

        var startRoom = new Room("Start", "Starting room", blockExitIfMonsterAlive: true);
        var nextRoom = new Room("Next", "Next room");
        startRoom.Monster = mockMonster.Object;
        startRoom.AddExit(Direction.North, nextRoom);

        var rooms = new Rooms(new List<Room> { startRoom, nextRoom }, startRoom);
        rooms.Move(Direction.North, mockPlayer.Object);

        mockPlayer.Verify(p => p.TakeDamage(100), Times.Once);
    }

}
