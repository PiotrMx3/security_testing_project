using Moq;

namespace DungeonGame.Test;

[TestFixture]
public class RoomsTests
{
    [Test]
    public void Move_Monster_alive_and_Block_exit_PlayerDies()
    {
        var mockPlayer = new Mock<IPlayer>();
        var mockMonster = new Mock<IMonster>();

        mockMonster.Setup(m => m.IsAlive).Returns(true);
        mockPlayer.SetupProperty(p => p.Health, 100);


        var startRoom = new Room("Start", "Starting room", blockExitIfMonsterAlive: true);
        var nextRoom = new Room("Next", "Next room");

        startRoom.Monster = mockMonster.Object;
        startRoom.AddExit(Direction.North, nextRoom);

        var rooms = new Rooms(new List<Room> { startRoom, nextRoom }, startRoom);

        rooms.Move(Direction.North, mockPlayer.Object);

        mockPlayer.Object.TakeDamage(100);
        Assert.AreEqual(0, mockPlayer.Object.Health);
        Assert.AreEqual(startRoom, rooms.CurrentRoom);
    }
    [Test]
    public void Move_MonsterAliveAndBlockExit_PlayerNotHurt()
    {
        var mockPlayer = new Mock<IPlayer>();
        var mockMonster = new Mock<IMonster>();
        mockMonster.Setup(m => m.IsAlive).Returns(false);
        mockPlayer.SetupProperty(p => p.Health, 100);
        var startRoom = new Room("Start", "Starting room", blockExitIfMonsterAlive: true);
        var nextRoom = new Room("Next", "Next room");

        startRoom.Monster = mockMonster.Object; 
        startRoom.AddExit(Direction.North, nextRoom);

        var rooms = new Rooms(new List<Room> { startRoom, nextRoom }, startRoom);
        rooms.Move(Direction.North, mockPlayer.Object);
        Assert.AreEqual(100, mockPlayer.Object.Health);
    }
    [Test]
    public void Move_NoExit_ReturnsFalse_AndCurrentRoomUnchanged()
    {
        var mockPlayer = new Mock<IPlayer>();
        var startRoom = new Room("Start", "Starting room");
        var rooms = new Rooms(new List<Room> { startRoom }, startRoom);

        bool result = rooms.Move(Direction.North, mockPlayer.Object);

        Assert.False(result);
        Assert.AreEqual(startRoom, rooms.CurrentRoom);
    }
    [Test]
    public void Move_LockedRoomWithoutKey_ReturnsFalse()
    {
        var mockPlayer = new Mock<IPlayer>();
        var mockInventory = new Mock<IInventory>();
        mockPlayer.Setup(p => p.Inventory).Returns(mockInventory.Object);
        mockInventory.Setup(i => i.HasKey("Golden Key")).Returns(false);

        var startRoom = new Room("Start", "Starting room");
        var lockedRoom = new Room("Vault", "Locked room", isLocked: true, requiredKeyName: "Golden Key");
        startRoom.AddExit(Direction.North, lockedRoom);

        var rooms = new Rooms(new List<Room> { startRoom, lockedRoom }, startRoom);

        bool result = rooms.Move(Direction.North, mockPlayer.Object);

        Assert.False(result);
        mockInventory.Verify(i => i.HasKey("Golden Key"), Times.Once);
    }
    public void Move_IntoDeadlyRoom_PlayerDies_ReturnsTrue()
    {
        var mockPlayer = new Mock<IPlayer>();
        var mockInventory = new Mock<IInventory>();
        mockPlayer.SetupProperty(p => p.Health, 100);
        mockPlayer.Setup(p => p.Inventory).Returns(mockInventory.Object);

        var startRoom = new Room("Start", "Starting room");
        var deadlyRoom = new Room("Trap", "A deadly trap", isDeadly: true);
        startRoom.AddExit(Direction.North, deadlyRoom);

        var rooms = new Rooms(new List<Room> { startRoom, deadlyRoom }, startRoom);

        bool result = rooms.Move(Direction.North, mockPlayer.Object);

        Assert.True(result);
        Assert.AreEqual(0, mockPlayer.Object.Health);
    }
}