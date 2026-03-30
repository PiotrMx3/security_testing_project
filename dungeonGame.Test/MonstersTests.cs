using DungeonGame;
namespace DungeonGame.Test;

[TestFixture]
public class MonstersTests
{
    [Test]
    public void TakeDamade_Reduce_Health_To_Zero()
    {
        Monster monster = new Monster("Goblin", 80, 15);
        monster.TakeDamage(70);
        Assert.AreEqual(0, monster.Health);
    }
    [Test]
    public void Take_Damage_Ignores_Negative_Damage()
    {
        Monster monster = new Monster("orc", 100,15);
        monster.TakeDamage (-25);
        Assert.AreEqual(100, monster.Health);
    }
}
