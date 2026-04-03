using DungeonGame;
using System.Security.Cryptography.X509Certificates;
namespace DungeonGame.Test;

[TestFixture]
public class MonstersTests
{
    [Test]
    public void Checking_Monster_Correct_name_is_Equal()
    {
        Monster monster = new Monster("Goblin", 100, 30);
        Assert.AreEqual("Goblin", monster.Name);
    }
    [Test]
    public void Taking_Damage_Reduces_Health()
    {
        Monster monster = new Monster("Goblin", 80, 15);
        monster.TakeDamage(30);
        Assert.AreEqual(50, monster.Health);
    }
   [Test]
    public void Taking_Damage_Exceeds_Maximum_Health()
    {
        Monster monster = new Monster("Goblin", 80, 15);
        monster.TakeDamage(120);
        Assert.AreEqual(0, monster.Health);
    }
    [Test]
    public void Taking_Damage_Ignores_Negative_Damage()
    {
        Monster monster = new Monster("orc", 100, 15);
        monster.TakeDamage(-25);
        Assert.AreEqual(100, monster.Health);
    }
    [Test]
    public void Monster_Deals_damage_To_The_Player()
    {
        Room romm = new Room("Testroom", "A room for testing", false, false, null, false);
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
