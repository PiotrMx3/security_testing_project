using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using DungeonGame.Models;

namespace DungeonGame.Test;

[TestFixture]
public class GameTest
{
    private Rooms rooms;
    private IRoom startRoom;
    private IRoom keyRoom;
    private IRoom swordRoom;
    private IRoom monsterRoom;
    private IRoom deathRoom;
    private IRoom winRoom;

    [SetUp]
    public void Setup()
    {
        // Rooms
        startRoom = new Room("Start", "Test");
        keyRoom = new Room("KeyRoom", "Test");
        swordRoom = new Room("SwordRoom", "Test");
        monsterRoom = new Room("MonsterRoom", "Test", blockExitIfMonsterAlive: true);
        deathRoom = new Room("DeathRoom", "Test", isDeadly: true);
        winRoom = new Room("WinRoom", "Test", isLocked: true, requiredKeyName: "Key");

        // Exits
        startRoom.AddExit(Direction.North, winRoom);
        startRoom.AddExit(Direction.East, keyRoom);
        startRoom.AddExit(Direction.South, swordRoom);
        startRoom.AddExit(Direction.West, deathRoom);

        swordRoom.AddExit(Direction.South, monsterRoom);
        monsterRoom.AddExit(Direction.North, swordRoom);
        keyRoom.AddExit(Direction.West, startRoom);

        // Items
        keyRoom.Items.Add(new Item("Key", "A rusty key.", ItemType.Key));
        swordRoom.Items.Add(new Item("Sword", "A sharp blade.", ItemType.Weapon));

        // Monster (echte instantie hier, gemockt in fight tests)
        monsterRoom.Monster = new Monster("Dragon", 50, 20, requiresWeapon: true);

        var allRooms = new List<IRoom> { startRoom, keyRoom, swordRoom, monsterRoom, deathRoom, winRoom };
        rooms = new Rooms(allRooms, startRoom);
    }

    // ---------------------------
    // Move tests (echt object)
    // ---------------------------
    [Test]
    public void MoveValidDirection_ShouldMovePlayer()
    {
        var player = new Player("Test", 100);
        var game = new Game(player, rooms);

        bool moved = game.Move("e");

        Assert.That(moved, Is.True);
        Assert.That(game.Rooms.CurrentRoom, Is.EqualTo(keyRoom));
    }

    [Test]
    public void MoveInvalidDirection_ShouldReturnFalse()
    {
        var player = new Player("Test", 100);
        var game = new Game(player, rooms);

        bool moved = game.Move("x");

        Assert.That(moved, Is.False);
    }

    [Test]
    public void MoveToDeadlyRoom_ShouldKillPlayer()
    {
        var player = new Player("Test", 100);
        var game = new Game(player, rooms);

        bool moved = game.Move("w"); // west = deadly room

        Assert.That(moved, Is.True);
        Assert.That(player.IsAlive, Is.False);
        Assert.That(player.Health, Is.EqualTo(0));
    }

    // ---------------------------
    // Take tests (echt object)
    // ---------------------------
    [Test]
    public void TakeExistingItem_ShouldAddToInventory()
    {
        var player = new Player("Test", 100);
        var game = new Game(player, rooms);

        game.Move("e"); // key room
        bool taken = game.Take("Key");

        Assert.That(taken, Is.True);
        Assert.That(player.Inventory.Contains("Key"), Is.True);
        Assert.That(game.Rooms.CurrentRoom.Items.Any(i => i.Name == "Key"), Is.False);
    }

    [Test]
    public void TakeItemWhenInventoryFull_ShouldFail()
    {
        var player = new Player("Test", 100);
        for (int i = 0; i < 5; i++)
            player.Inventory.Add(new Item($"Item{i}", "Test", ItemType.Consumable));

        var game = new Game(player, rooms);
        game.Move("e"); // key room
        bool taken = game.Take("Key");

        Assert.That(taken, Is.False);
        Assert.That(player.Inventory.Contains("Key"), Is.False);
    }

    // ---------------------------
    // Fight tests (mock Player & Monster)
    // ---------------------------
    [Test]
    public void FightMonsterWithWeapon_PlayerWins()
    {
        // Mock monster om te controleren interactie
        var mockMonster = new Mock<IMonster>();
        mockMonster.SetupProperty(m => m.Health, 50);
        mockMonster.SetupProperty(m => m.Name, "Dragon");
        mockMonster.Setup(m => m.RequiresWeapon).Returns(true);
        mockMonster.Setup(m => m.TakeDamage(It.IsAny<int>()))
                   .Callback<int>(dmg => mockMonster.Object.Health -= dmg);
        mockMonster.Setup(m => m.IsAlive).Returns(() => mockMonster.Object.Health > 0);

        // Mock player
        var mockInventory = new Mock<IInventory>();
        mockInventory.Setup(i => i.HasWeapon()).Returns(true);

        var mockPlayer = new Mock<IPlayer>();
        mockPlayer.SetupProperty(p => p.Health, 100);
        mockPlayer.Setup(p => p.Inventory).Returns(mockInventory.Object);
        mockPlayer.Setup(p => p.TakeDamage(It.IsAny<int>()))
                  .Callback<int>(dmg => mockPlayer.Object.Health -= dmg);
        mockPlayer.Setup(p => p.IsAlive).Returns(() => mockPlayer.Object.Health > 0);

        var testRoom = new Room("MonsterRoom", "Test", blockExitIfMonsterAlive: true);
        testRoom.Monster = mockMonster.Object;

        var rooms = new Rooms(new List<IRoom> { testRoom }, testRoom);
        var game = new Game(mockPlayer.Object, rooms);

        bool result = game.Fight();

        Assert.That(result, Is.True);
        mockMonster.Verify(m => m.TakeDamage(It.IsAny<int>()), Times.AtLeastOnce);
    }

    [Test]
    public void FightMonsterRequiresWeapon_PlayerWithoutWeapon_Dies()
    {
        // Mock monster om te controleren interactie
        var mockMonster = new Mock<IMonster>();
        mockMonster.Setup(m => m.IsAlive).Returns(true);
        mockMonster.Setup(m => m.RequiresWeapon).Returns(true);
        mockMonster.Setup(m => m.TakeDamage(It.IsAny<int>()));
        mockMonster.SetupProperty(m => m.Health, 50);
        mockMonster.SetupProperty(m => m.Name, "Dragon");

        var mockInventory = new Mock<IInventory>();
        mockInventory.Setup(i => i.HasWeapon()).Returns(false);

        var mockPlayer = new Mock<IPlayer>();
        mockPlayer.Setup(p => p.Inventory).Returns(mockInventory.Object);
        mockPlayer.SetupProperty(p => p.Health, 100);
        mockPlayer.Setup(p => p.IsAlive).Returns(true);

        var testRoom = new Room("MonsterRoom", "Test", blockExitIfMonsterAlive: true);
        testRoom.Monster = mockMonster.Object;

        var rooms = new Rooms(new List<IRoom> { testRoom }, testRoom);
        var game = new Game(mockPlayer.Object, rooms);

        bool result = game.Fight();

        Assert.That(result, Is.False);
        Assert.That(mockPlayer.Object.Health, Is.EqualTo(0));
    }

    // ---------------------------
    // CheckWin / IsGameOver tests
    // ---------------------------
    [Test]
    public void CheckWin_InWinRoom_ShouldSetWinner()
    {
        var player = new Player("Test", 100);
        var game = new Game(player, rooms);

        // Pak key eerst
        game.Move("e");
        game.Take("Key");
        game.Move("w"); // terug naar start
        game.Move("n"); // naar win room

        bool won = game.CheckWin();

        Assert.That(won, Is.True);
        Assert.That(player.IsWinner, Is.True);
    }

    [Test]
    public void IsGameOver_PlayerDeadOrWinner_ShouldReturnTrue()
    {
        var player = new Player("Test", 100) { Health = 0 };
        var game = new Game(player, rooms);
        Assert.That(game.IsGameOver(), Is.True);

        player.Health = 100;
        player.IsWinner = true;
        Assert.That(game.IsGameOver(), Is.True);
    }
}
