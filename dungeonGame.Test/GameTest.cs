using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DungeonGame.Test;

[TestFixture]
public class GameTest
{
    private Game game;
    // Gebruik interfaces voor mocking
    private Mock<IPlayer> mockPlayer;
    private Mock<IInventory> mockInventory;
    // Echte objecten voor eenvoudige klassen
    private Room entrance;
    private Room armory;
    private Room crypt;
    private Room pillarHall;
    private Room monsterRoom;
    private Room treasureRoom;
    [SetUp]
    public void Setup()
    {
        // Setup mocks
        mockInventory = new Mock<IInventory>();
        mockPlayer = new Mock<IPlayer>();

        // Setup real rooms
        entrance = new Room("Entrance", "Test");
        armory = new Room("Armory", "Test");
        crypt = new Room("Crypt", "Test");

        entrance.AddExit("east", armory);
        armory.AddExit("east", crypt);

        // Setup default behavior
        mockPlayer.Setup(p => p.Inventory).Returns(mockInventory.Object);
        mockPlayer.Setup(p => p.CurrentRoom).Returns(entrance);
        mockPlayer.Setup(p => p.IsAlive).Returns(true);

        game = new Game(mockPlayer.Object,entrance,armory,crypt,null,null,null);
    }

    [Test]
    public void MoveValidDirectionShouldCallPlayerMove()
    {
        // Arrange
        mockPlayer.Setup(p => p.Move("east")).Returns(true);

        // Act
        bool result = game.Move("east");

        // Assert
        Assert.That(result, Is.True);
        mockPlayer.Verify(p => p.Move("east"), Times.Once);

    }

    [Test]
    public void PickUpExistingItemShouldCallPlayerPickUpItem()
    {
        // Arrange
        var testItem = new Item("Sword", "Test", ItemType.Weapon);
        entrance.Items.Add(testItem);
        mockPlayer.Setup(p => p.PickUpItem(It.IsAny<Item>())).Returns(true);

        // Act
        bool result = game.PickUp("Sword");

        // Assert
        Assert.That(result, Is.True);
        mockPlayer.Verify(p => p.PickUpItem(It.Is<Item>(i => i.Name == "Sword")), Times.Once);
    }
    [Test]
    public void FightNoMonsterShouldNotModifyPlayerHealt()
    {
        // Arrange
        mockPlayer.Setup(p => p.CurrentRoom).Returns(entrance);

        // Act
        bool result = game.Fight();

        // Assert
        Assert.That(result, Is.False);
        mockPlayer.Verify(p => p.TakeDamage(It.IsAny<int>()), Times.Never);
    }
    [Test]
    public void FightMonsterRequiresWeaponPlayerHasWeaponShouldKillMonster()
    {
        // Arrange
        var monsterRoom = new Room("Monster", "Test");
        var monster = new Monster("Troll", 50, 25, requiresWeapon: true);
        monsterRoom.Monster = monster;

        mockPlayer.Setup(p => p.CurrentRoom).Returns(monsterRoom);
        mockInventory.Setup(i => i.HasWeapon()).Returns(true);
        mockPlayer.Setup(p => p.IsAlive).Returns(true);

        // Setup health zodat speler overleeft
        mockPlayer.SetupProperty(p => p.Health);
        mockPlayer.Setup(p => p.Health).Returns(100);

        // Act
        bool result = game.Fight();

        // Assert
        Assert.That(result, Is.True);
        Assert.That(monster.IsAlive, Is.False);
    }

    [Test]
    public void FightMonsterRequiresWeaponsPlayerNoWeaponShouldKillPlayer()
    {
        // Arrange
        var monsterRoom = new Room("Monster", "Test");
        var monster = new Monster("Troll", 50,25, requiresWeapon: true);
        monsterRoom.Monster = monster;

        mockPlayer.Setup(p => p.CurrentRoom).Returns(monsterRoom);
        mockInventory.Setup(i => i.HasWeapon()).Returns(false);

        // Act
        bool result = game.Fight();

        // Assert
        Assert.That(result, Is.False);
        mockPlayer.VerifySet(p => p.Health = 0, Times.Once);
    }

    [Test]
    public void CheckWinInTreasureRoomWithThreasureShouldSetWinner()
    {
        // Arrange
        var treasureRoom = new Room("TreasureRoom", "Test");
        mockPlayer.Setup(p => p.CurrentRoom).Returns(treasureRoom);
        mockInventory.Setup(i => i.Contains("Treasure")).Returns(true);

        // Act
        bool result = game.CheckWin();

        // Assert
        Assert.That(result, Is.True);
        mockPlayer.VerifySet(p => p.IsWinner = true, Times.Once);
    }

    [Test]
    public void CheckWinNotInTreasureRoomShouldNotSetWinner()
    {
        // Arrange
        mockPlayer.Setup(p => p.CurrentRoom).Returns(entrance);

        // Act
        bool result = game.CheckWin();

        // Assert
        Assert.That(result, Is.False);
        mockPlayer.VerifySet(p => p.IsWinner = true, Times.Never);
    }

    [Test]
    public void IsGameOverWhenPlayerDeadShouldReturntTrue()
    {
        // Arrange
        mockPlayer.Setup(p => p.IsAlive).Returns(false);

        // Act
        bool result = game.IsGameOver();

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsGameOverWhenPlayerWinnerShouldReturnTrue() 
    {
        // Arrange
        mockPlayer.Setup(p => p.IsAlive).Returns(true);
        mockPlayer.Setup(p => p.IsWinner).Returns(true);   

        // Act
        bool result = game.IsGameOver();

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void MoveToLockedRoomWithoutKeyShouldReturnFalse()
    {
        // Arrange
        var lockedRoom = new Room("Locked", "Test", isLocked: true, requiredKeyName: "GoldKey");
        entrance.AddExit("north", lockedRoom);

        // Gebruik echte player
        var realPlayer = new Player("Test", 100, entrance);

        // Act
        bool result = realPlayer.Move("north");

        // Assert
        Assert.That(result, Is.False);
        Assert.That(realPlayer.CurrentRoom, Is.EqualTo(entrance));
    }

    [Test]
    public void MoveToLockedRoomWithKeyShouldReturnTrue()
    {
        // Arrange
        var lockedRoom = new Room("Locked", "Test", isLocked: true, requiredKeyName: "GoldKey");
        entrance.AddExit("north", lockedRoom);

        var realPlayer = new Player("Test", 100, entrance);
        var key = new Item("GoldKey", "Test", ItemType.Key);
        realPlayer.Inventory.Add(key);

        // Act
        bool result = realPlayer.Move("north");

        // Assert
        Assert.That(result, Is.True);
        Assert.That(realPlayer.CurrentRoom, Is.EqualTo(lockedRoom));
    }
    [Test]
    public void FightMultipleHitsShouldVerifyDamageApplied()
    {
        // Arrange
        var monsterRoom = new Room("Monster", "Test");
        var monster = new Monster("Goblin", 30, 10);
        monsterRoom.Monster = monster;

        mockPlayer.Setup(p => p.CurrentRoom).Returns(monsterRoom);
        mockPlayer.Setup(p => p.IsAlive).Returns(true);

        // Act
        game.Fight();

        // Assert
        // Monster should have taken damage
        Assert.That(monster.IsAlive, Is.False);
    }
}
