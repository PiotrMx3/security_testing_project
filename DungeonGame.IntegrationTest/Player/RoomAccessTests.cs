namespace DungeonGame.IntegrationTest;
[TestFixture]
public class RoomAccessTests
{
    private IPlayer _player;
    private IRoom _startRoom;
    private IRoom _keyRoom;
    private IRoom _winRoom;
    private IRooms _world;

    [SetUp]
    public void Setup()
    {
        _player = new Player("Held", 100);
        
        _startRoom = new Room("Start", "Het begin");
        _keyRoom = new Room("Sleutelkamer", "Hier ligt de sleutel");
        _winRoom = new Room("Eindkamer", "De uitgang!", isLocked: true, requiredKeyName: "Gouden sleutel");

        _startRoom.AddExit(Direction.East, _keyRoom);
        _startRoom.AddExit(Direction.North, _winRoom);
        _keyRoom.AddExit(Direction.North, _startRoom);

        var allRooms = new List<IRoom> { _startRoom, _keyRoom, _winRoom };
        _world = new Rooms(allRooms, _startRoom);
    }

    [Test]
    public void Move_ToWinRoomWithoutKey_ReturnsFalseAndPlayerStaysInStartRoom()
    {
        // 1. ARRANGE - Niet nodig in deze test (SetUp is voldoende)
        // 2. ACT
        bool result = _world.Move(Direction.North, _player);
        // 3. ASSERT
        Assert.Multiple(() => 
        {
            Assert.That(result, Is.False, "Zonder sleutel mag je deze kamer niet binnen!");
            Assert.That(_world.CurrentRoom, Is.EqualTo(_startRoom), "Speler moet nog steeds in de startkamer zijn.");
        });
    }
    [Test]
    public void Move_ToWinRoomWithKey_ReturnsTrueAndPlayerEntersWinRoom()
    {
        // 1. ARRANGE - We zorgen dat er een sleutel in de de _keyRoom aanwezig is
        IItem key = new Item("Gouden sleutel", "Een glimmende gouden sleutel", ItemType.Key);
        _keyRoom.Items.Add(key);
        // 2. ACT - We gaan naar de keyroom en pakken de key en voegen deze toe aan onze Inventory.
        // -------- We keren terug naar de start kamer en proberen de _winRoom binnen te gaan. 
        _world.Move(Direction.East, _player);
        IItem pickedUp = _world.CurrentRoom.TakeItem("Gouden sleutel");
        if (pickedUp != null)
        {
            _player.Inventory.Add(pickedUp);
        }
        _world.Move(Direction.North, _player);
        bool result = _world.Move(Direction.North, _player);
        // 3. ASSERT
        Assert.That(result, Is.True, "Met een sleutel kunnen we de kamer binnen");
        Assert.That(_world.CurrentRoom, Is.EqualTo(_winRoom));
    }
}
