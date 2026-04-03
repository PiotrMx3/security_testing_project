using DungeonGame;
using System.Security.Cryptography.X509Certificates;
namespace DungeonGame.Test;

[TestFixture]
public class MonstersTests
{
    [Test]
    public void Monster_Taking_Damage_To_Zero()
    {
        Monster monster = new Monster("Goblin", 80, 15);
        monster.TakeDamage(30);
        Assert.AreEqual(50, monster.Health);
        monster.TakeDamage(50);
        Assert.AreEqual(0, monster.Health);
    }
    [Test]
    public void Taking_Damage_Ignores_Negative_Damage()
    {
        Monster monster = new Monster("orc", 100,15);
        monster.TakeDamage(-25);
        Assert.AreEqual(100, monster.Health);
    }
    [Test]
    public void Monster_Deals_damage_To_The_Player()
    {
        Player player = new Player("Hero", 100);
        Monster monster = new Monster("Orc", 50, 20);

        int previousHealth = player.Health;
        //act
        monster.Attack(player);

        //assert
        Assert.IsTrue(player.Health < previousHealth);
    }
    [Test]
    public void Monster_Requires_Weapon()
    {
        Monster monsternWithWeapon = new Monster("Dragon", 200, 30, true);
        Monster monsterWitoutWeapon = new Monster("Slime", 20, 5);

        Assert.IsTrue(monsternWithWeapon.RequiresWeapon);
        Assert.IsFalse(monsterWitoutWeapon.RequiresWeapon);

        
    }
    [Test]
    public static void Monster_Health_ReachesZero()
    {
        Monster monster = new Monster("Goblin", 30, 50);
        monster.TakeDamage(30);
        Assert.IsFalse(monster.IsAlive);
    }
}
