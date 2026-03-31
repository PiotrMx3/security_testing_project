using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace DungeonGame.Test;

[TestFixture]
public class GameTest
{
    // Gebruik interfaces voor mocking
    private Mock<IPlayer> mockPlayer;
    private Mock<IInventory> mockInventory;
    // Echte objecten voor eenvoudige klassen
    private Rooms rooms;
    private Room startRoom;
    private Room keyRoom;
    private Room swordRoom;
    private Room monsterRoom;
    private Room deathRoom;
    private Room winRoom;

    [SetUp]
    public void Setup()
    {
        // Setup mocks
        mockInventory = new Mock<IInventory>();
        mockPlayer = new Mock<IPlayer>();

        // Setup default behavior
        mockPlayer.Setup(p => p.Inventory).Returns(mockInventory.Object);
        mockPlayer.Setup(p => p.IsAlive).Returns(true);
        mockPlayer.Setup(p => p.Health).Returns(100);
        mockPlayer.SetupProperty(p => p.Health);
        mockPlayer.SetupProperty(p => p.IsWinner);

        // Setup real rooms volgens GameSetup
        startRoom = new Room("Start", "Test");
        keyRoom = new Room("KeyRoom", "Test");
        swordRoom = new Room("SwordRoom", "Test");
        monsterRoom = new Room("MonsterRoom", "Test", blockExitIfMonsterAlive: true);
        deathRoom = new Room("DeathRoom", "Test", isDeadly: true);
        winRoom = new Room("WinRoom", "Test", isLocked: true, requiredKeyName: "Key");

        // Setup exits
        startRoom.AddExit(Direction.North, winRoom);
        startRoom.AddExit(Direction.East, keyRoom);
        startRoom.AddExit(Direction.South, swordRoom);
        startRoom.AddExit(Direction.West, deathRoom);

        swordRoom.AddExit(Direction.South, monsterRoom);
        monsterRoom.AddExit(Direction.North, swordRoom);
        keyRoom.AddExit(Direction.West, startRoom);

        // ITEMS - VOEG DEZE TOE!
        keyRoom.Items.Add(new Item("Key", "A rusty key.", ItemType.Key));
        swordRoom.Items.Add(new Item("Sword", "A sharp blade.", ItemType.Weapon));

        // Monster
        monsterRoom.Monster = new Monster("Dragon", 50, 20, requiresWeapon: true);

        var allRooms = new List<Room> { startRoom, keyRoom, swordRoom, monsterRoom, deathRoom, winRoom };
        rooms = new Rooms(allRooms, startRoom);
    }


    [Test]
    public void MoveValidDirectionWithRealPlayer_ShouldMovePlayer()
    {
        // Arrange
        var realPlayer = new Player("Test", 100);
        var testGame = new Game(realPlayer, rooms);

        // Act
        bool result = testGame.Move("e");

        // Assert
        Assert.That(result, Is.True);
        Assert.That(testGame.Rooms.CurrentRoom, Is.EqualTo(keyRoom));
    }

    [Test]
    public void MoveInvalidDirectionShouldReturnFalse()
    {
        // Arrange
        var realPlayer = new Player("Test", 100);
        var testGame = new Game(realPlayer, rooms);

        // Act
        bool result = testGame.Move("x");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]

    public void Take_ExistingItemShouldAddToInventory()
    {
        // Arrange
        var realPlayer = new Player("Test", 100);
        var testGame = new Game(realPlayer, rooms);

        // Ga naar key room
        testGame.Move("e");

        // Act
        bool result = testGame.Take("Key");

        // Assert
        Assert.That(result, Is.True, "Take should return true");
        Assert.That(realPlayer.Inventory.Contains("Key"), Is.True, "Key should be in inventory");
        Assert.That(testGame.Rooms.CurrentRoom.Items.Any(i => i.Name == "Key"), Is.False, "Key should be removed from room");
    }

    [Test]
    public void TakeNonExistingItemShouldReturnFalse()
    {
        // Arrange
        var realPlayer = new Player("Test", 100);
        var testGame = new Game(realPlayer, rooms);

        // Act
        bool result = testGame.Take("NonExisting");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void TakeWhenInventoryFullShouldReturnFalse()
    {
        // Arrange
        var realPlayer = new Player("Test", 100);
        // Vul inventory
        for (int i = 0; i < 5; i++)
        {
            realPlayer.Inventory.Add(new Item($"Item{i}", "Test", ItemType.Consumable));
        }
        var testGame = new Game(realPlayer, rooms);

        // Act
        bool result = testGame.Take("Key");

        // Assert
        Assert.That(result, Is.False);
        Assert.That(realPlayer.Inventory.Contains("Key"), Is.False);
    }

    [Test]
    public void Fight_NoMonster_ShouldReturnFalse()
    {
        // Arrange
        var realPlayer = new Player("Test", 100);
        var testGame = new Game(realPlayer, rooms);
        testGame.Rooms.CurrentRoom = startRoom;

        // Act
        bool result = testGame.Fight();

        // Assert
        Assert.That(result, Is.False);
    }
    [Test]
    public void FightMonsterRequiresWeaponPlayerHasWeaponShouldKillMonster()
    {
        // Arrange
        var realPlayer = new Player("Test", 100);
        var testGame = new Game(realPlayer, rooms);

        // Ga naar sword room en pak zwaard
        testGame.Move("s");
        testGame.Take("Sword");

        // Ga naar monster room
        testGame.Move("s");

        // Act
        bool result = testGame.Fight();

        // Assert
        Assert.That(result, Is.True);
        Assert.That(monsterRoom.Monster.IsAlive, Is.False);
        Assert.That(realPlayer.IsAlive, Is.True);
    }

    [Test]
    public void FightMonsterRequiresWeaponsPlayerNoWeaponShouldKillPlayer()
    {
        // Arrange
        var realPlayer = new Player("Test", 100);
        var testGame = new Game(realPlayer, rooms);

        // Ga direct naar monster room zonder wapen
        testGame.Move("s");
        testGame.Move("s");

        // Act
        bool result = testGame.Fight();

        // Assert
        Assert.That(result, Is.False);
        Assert.That(realPlayer.IsAlive, Is.False);
        Assert.That(realPlayer.Health, Is.EqualTo(0));
    }

    [Test]
    public void FightMonsterWithoutWeaponRequirementShouldKillMonster()
    {
        // Arrange
        var realPlayer = new Player("Test", 100);
        // Maak een kamer met een zwak monster
        var weakMonsterRoom = new Room("WeakRoom", "Test");
        var weakMonster = new Monster("Goblin", 10, 5);
        weakMonsterRoom.Monster = weakMonster;

        var allRooms = new List<Room> { startRoom, weakMonsterRoom };
        var testRooms = new Rooms(allRooms, weakMonsterRoom);
        var testGame = new Game(realPlayer, testRooms);

        // Act
        bool result = testGame.Fight();

        // Assert
        Assert.That(result, Is.True);
        Assert.That(weakMonster.IsAlive, Is.False);
    }

    [Test]
    public void CheckWin_NotInWinRoom_ShouldNotSetWinner()
    {
        // Arrange
        var realPlayer = new Player("Test", 100);
        var testGame = new Game(realPlayer, rooms);

        // Act
        bool result = testGame.CheckWin();

        // Assert
        Assert.That(result, Is.False);
        Assert.That(realPlayer.IsWinner, Is.False);
    }

    [Test]
    public void CheckWin_InWinRoom_ShouldSetWinner()
    {
        // Arrange
        var realPlayer = new Player("Test", 100);
        var testGame = new Game(realPlayer, rooms);

        // Stap 1: Ga naar key room
        bool moveEast = testGame.Move("e");
        Assert.That(moveEast, Is.True, "Should move east");
        Assert.That(testGame.Rooms.CurrentRoom.Name, Is.EqualTo("KeyRoom"), "Should be in KeyRoom");

        // Stap 2: Controleer of key in de kamer is
        var keyInRoom = testGame.Rooms.CurrentRoom.Items.Any(i => i.Name == "Key");
        Assert.That(keyInRoom, Is.True, "Key should be in KeyRoom");

        // Stap 3: Pak de key
        bool takeKey = testGame.Take("Key");
        Assert.That(takeKey, Is.True, "Should take key");
        Assert.That(realPlayer.Inventory.Contains("Key"), Is.True, "Key should be in inventory");

        // Stap 4: Ga terug naar start
        bool moveWest = testGame.Move("w");
        Assert.That(moveWest, Is.True, "Should move west");
        Assert.That(testGame.Rooms.CurrentRoom.Name, Is.EqualTo("Start"), "Should be back in Start");

        // Stap 5: Ga naar win room
        bool moveNorth = testGame.Move("n");
        Assert.That(moveNorth, Is.True, "Should move north to win room");
        Assert.That(testGame.Rooms.CurrentRoom.Name, Is.EqualTo("WinRoom"), "Should be in WinRoom");

        // Stap 6: Check win
        bool result = testGame.CheckWin();

        // Assert
        Assert.That(result, Is.True);
        Assert.That(realPlayer.IsWinner, Is.True);
    }

    [Test]
    public void IsGameOverWhenPlayerDeadShouldReturntTrue()
    {
        // Arrange
        var realPlayer = new Player("Test", 100);
        var testGame = new Game(realPlayer, rooms);
        realPlayer.Health = 0;  // Maak de speler dood

        // Act
        bool result = testGame.IsGameOver();

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsGameOverWhenPlayerWinnerShouldReturnTrue() 
    {
        // Arrange
        var realPlayer = new Player("Test", 100);
        var testGame = new Game(realPlayer, rooms);
        realPlayer.IsWinner = true;

        // Act
        bool result = testGame.IsGameOver();

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void MoveToLockedRoomWithoutKeyShouldReturnFalse()
    {
        // Arrange
        var realPlayer = new Player("Test", 100);
        var testGame = new Game(realPlayer, rooms);

        // Act - probeer naar win room te gaan zonder key
        bool result = testGame.Move("n");

        // Assert
        Assert.That(result, Is.False);
        Assert.That(testGame.Rooms.CurrentRoom, Is.EqualTo(startRoom));
    }

    [Test]
    public void MoveToLockedRoomWithKeyShouldReturnTrue()
    {
        // Arrange
        var realPlayer = new Player("Test", 100);
        var testGame = new Game(realPlayer, rooms);

        // Eerst key pakken
        testGame.Move("e");
        testGame.Take("Key");
        testGame.Move("w");

        // Act - probeer naar win room te gaan
        bool result = testGame.Move("n");

        // Assert
        Assert.That(result, Is.True);
        Assert.That(testGame.Rooms.CurrentRoom, Is.EqualTo(winRoom));
    }

    [Test]
    public void MoveToDeadlyRoom_ShouldKillPlayer()
    {
        // Arrange
        var realPlayer = new Player("Test", 100);
        var testGame = new Game(realPlayer, rooms);

        // Act
        bool result = testGame.Move("w");

        // Assert
        Assert.That(result, Is.True);
        Assert.That(realPlayer.IsAlive, Is.False);
        Assert.That(realPlayer.Health, Is.EqualTo(0));
    }

    [Test]
    public void MoveFromMonsterRoomWithAliveMonster_ShouldKillPlayer()
    {
        // Arrange
        var realPlayer = new Player("Test", 100);
        var testGame = new Game(realPlayer, rooms);

        // Ga naar monster room (via sword room)
        testGame.Move("s");
        testGame.Move("s");

        // Probeer terug te gaan terwijl monster leeft
        bool result = testGame.Move("n");

        // Assert
        Assert.That(result, Is.False);
        Assert.That(realPlayer.IsAlive, Is.False);
        Assert.That(realPlayer.Health, Is.EqualTo(0));
    }

    [Test]
    public void IsGameOver_WhenPlayerDead_ShouldReturnTrue()
    {
        // Arrange
        var realPlayer = new Player("Test", 100);
        realPlayer.Health = 0;
        var testGame = new Game(realPlayer, rooms);

        // Act
        bool result = testGame.IsGameOver();

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsGameOver_WhenPlayerWinner_ShouldReturnTrue()
    {
        // Arrange
        var realPlayer = new Player("Test", 100);
        realPlayer.IsWinner = true;
        var testGame = new Game(realPlayer, rooms);

        // Act
        bool result = testGame.IsGameOver();

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Help_ShouldReturnNonEmptyString()
    {
        // Arrange
        var realPlayer = new Player("Test", 100);
        var testGame = new Game(realPlayer, rooms);

        // Act
        string help = testGame.Help();

        // Assert
        Assert.That(help, Is.Not.Null);
        Assert.That(help, Does.Contain("Available commands"));
    }

    [Test]
    public void Look_ShouldReturnRoomInfo()
    {
        // Arrange
        var realPlayer = new Player("Test", 100);
        var testGame = new Game(realPlayer, rooms);

        // Act
        string look = testGame.Look();

        // Assert
        Assert.That(look, Is.Not.Null);
        Assert.That(look, Does.Contain("Start"));
    }


    [Test]
    public void ShowInventory_ShouldShowPlayerItems()
    {
        // Arrange
        var realPlayer = new Player("Test", 100);
        var testGame = new Game(realPlayer, rooms);
        realPlayer.Inventory.Add(new Item("TestItem", "Test", ItemType.Consumable));

        // Act
        string inventory = testGame.ShowInventory();

        // Assert
        Assert.That(inventory, Does.Contain("TestItem"));
    }
}
