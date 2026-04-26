using DungeonGame.Models.Interfaces;
using Moq;

namespace DungeonGame.IntegrationTest;

public class CombatTests
{
    private Game _sut;
    private Player _player;
    private Rooms _rooms;
    private Mock<IRandom> _rnd;
    private IRoom _start;

    [SetUp]
    public void Setup()
    {
        _player = new Player("test", 100);
        _rnd = new Mock<IRandom>();
        _rooms = GameSetup.CreateWorld();
        _start = _rooms.AllRooms.FirstOrDefault(r => r.Name == "Start")!;
        _sut = new Game(_player, new Rooms(_rooms.AllRooms, _start, _rnd.Object));

    }

    [Test]
    public void PlayerWithSword_FightingMonster_Wins()
    {
        //Arange
        int HighDmgRoll = 30; 
        _sut.Move("s");
        IItem? weapon = _sut.Rooms.CurrentRoom.TakeItem("Sword");
        _sut.Player.Inventory.Add(weapon!);
        _sut.Move("s");
        _rnd.Setup(r => r.Next()).Returns(HighDmgRoll);

        //Act
        bool actual = _sut.Rooms.Fight(_sut.Player);

        //Assert 
        Assert.That(actual, Is.EqualTo(true));
        Assert.That(_sut.Rooms.CurrentRoom.Monster!.IsAlive, Is.EqualTo(false));
        Assert.That(_sut.Player.IsAlive, Is.EqualTo(true));

    }

    [Test]

    public void PlayerWithoutSword_FightingMonster_Lose()
    {
        //Arange
        _sut.Move("s");
        _sut.Move("s");

        //Act
        bool actual = _sut.Rooms.Fight(_sut.Player);

        //Assert 
        Assert.That(actual, Is.EqualTo(false));
        Assert.That(_sut.Rooms.CurrentRoom.Monster!.IsAlive, Is.EqualTo(true));
        Assert.That(_sut.Player.IsAlive, Is.EqualTo(false));
    }

}
