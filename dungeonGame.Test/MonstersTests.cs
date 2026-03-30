using DungeonGame;
namespace DungeonGame.Test;

[TestFixture]
public class MonstersTests
{
    [Test]
    public void TakeDamade_Reduce_Health_To_Zero()
    {
        Monster monster = new Monster("Goblin", 80, 15);
        monster.TakeDamage(100);
        Assert.AreEqual(100, monster.Health);
    }
}
