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
    private Mock<IPlayer> mockPlayer;
    private Mock<IRooms> mockRooms;
    private Mock<IInventory> mockInventory;
    private Mock<IRoom> mockRoom;

    [SetUp]
    public void Setup()
    {
        mockPlayer = new Mock<IPlayer>();
        mockRooms = new Mock<IRooms>();
        mockInventory = new Mock<IInventory>();
        mockRoom = new Mock<IRoom>();

        mockPlayer.Setup(p => p.Inventory).Returns(mockInventory.Object);
        mockRooms.Setup(r => r.CurrentRoom).Returns(mockRoom.Object);
    }

    [Test]
    public void Move_InvalidDirection_ReturnsFalse_AndDoesNotCallRoomsMove()
    {
        var game = new Game(mockPlayer.Object, mockRooms.Object);

        bool result = game.Move("x");

        Assert.That(result, Is.False);
        mockRooms.Verify(r => r.Move(It.IsAny<Direction>(), It.IsAny<IPlayer>()), Times.Never);
    }

    [Test]
    public void Move_ValidDirection_CallsRoomsMove()
    {
        mockRooms
            .Setup(r => r.Move(Direction.East, mockPlayer.Object))
            .Returns(true);

        var game = new Game(mockPlayer.Object, mockRooms.Object);

        bool result = game.Move("e");

        Assert.That(result, Is.True);
        mockRooms.Verify(r => r.Move(Direction.East, mockPlayer.Object), Times.Once);
    }

    [Test]
    public void Take_ExistingItem_AddsItemToInventory_ReturnsTrue()
    {
        var item = new Item("Key", "A rusty key.", ItemType.Key);

        mockRoom
            .Setup(r => r.TakeItem("Key"))
            .Returns(item);

        mockInventory
            .Setup(i => i.Add(item))
            .Returns(true);

        var game = new Game(mockPlayer.Object, mockRooms.Object);

        bool result = game.Take("Key");

        Assert.That(result, Is.True);
        mockRoom.Verify(r => r.TakeItem("Key"), Times.Once);
        mockInventory.Verify(i => i.Add(item), Times.Once);
    }

    [Test]
    public void Take_ItemDoesNotExist_ReturnsFalse()
    {
        mockRoom
            .Setup(r => r.TakeItem("Key"))
            .Returns((IItem?)null);

        var game = new Game(mockPlayer.Object, mockRooms.Object);

        bool result = game.Take("Key");

        Assert.That(result, Is.False);
        mockInventory.Verify(i => i.Add(It.IsAny<IItem>()), Times.Never);
    }

    [Test]
    public void Take_InventoryFull_ReturnsFalse_AndPutsItemBackInRoom()
    {
        var item = new Item("Key", "A rusty key.", ItemType.Key);
        var roomItems = new List<IItem>();

        mockRoom
            .Setup(r => r.TakeItem("Key"))
            .Returns(item);

        mockRoom
            .Setup(r => r.Items)
            .Returns(roomItems);

        mockInventory
            .Setup(i => i.Add(item))
            .Returns(false);

        var game = new Game(mockPlayer.Object, mockRooms.Object);

        bool result = game.Take("Key");

        Assert.That(result, Is.False);
        Assert.That(roomItems, Does.Contain(item));
    }

    [Test]
    public void Fight_CallsRoomsFight()
    {
        mockRooms
            .Setup(r => r.Fight(mockPlayer.Object))
            .Returns(true);

        var game = new Game(mockPlayer.Object, mockRooms.Object);

        bool result = game.Fight();

        Assert.That(result, Is.True);
        mockRooms.Verify(r => r.Fight(mockPlayer.Object), Times.Once);
    }

    [Test]
    public void CheckWin_WhenCurrentRoomIsWinRoom_SetsWinnerAndReturnsTrue()
    {
        mockRoom.Setup(r => r.Name).Returns("WinRoom");
        mockPlayer.SetupProperty(p => p.IsWinner, false);

        var game = new Game(mockPlayer.Object, mockRooms.Object);

        bool result = game.CheckWin();

        Assert.That(result, Is.True);
        Assert.That(mockPlayer.Object.IsWinner, Is.True);
    }

    [Test]
    public void CheckWin_WhenCurrentRoomIsNotWinRoom_ReturnsFalse()
    {
        mockRoom.Setup(r => r.Name).Returns("Start");
        mockPlayer.SetupProperty(p => p.IsWinner, false);

        var game = new Game(mockPlayer.Object, mockRooms.Object);

        bool result = game.CheckWin();

        Assert.That(result, Is.False);
        Assert.That(mockPlayer.Object.IsWinner, Is.False);
    }

    [Test]
    public void IsGameOver_PlayerDead_ReturnsTrue()
    {
        mockPlayer.Setup(p => p.IsAlive).Returns(false);
        mockPlayer.Setup(p => p.IsWinner).Returns(false);

        var game = new Game(mockPlayer.Object, mockRooms.Object);

        Assert.That(game.IsGameOver(), Is.True);
    }

    [Test]
    public void IsGameOver_PlayerWinner_ReturnsTrue()
    {
        mockPlayer.Setup(p => p.IsAlive).Returns(true);
        mockPlayer.Setup(p => p.IsWinner).Returns(true);

        var game = new Game(mockPlayer.Object, mockRooms.Object);

        Assert.That(game.IsGameOver(), Is.True);
    }

    [Test]
    public void IsGameOver_PlayerAliveAndNotWinner_ReturnsFalse()
    {
        mockPlayer.Setup(p => p.IsAlive).Returns(true);
        mockPlayer.Setup(p => p.IsWinner).Returns(false);

        var game = new Game(mockPlayer.Object, mockRooms.Object);

        Assert.That(game.IsGameOver(), Is.False);
    }
}
