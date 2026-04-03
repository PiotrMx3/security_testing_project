using Moq;
using DungeonGame;

namespace Testing
{
    [TestFixture]
public class PlayerTest
    {
        [Test]
        public void Checking_for_correct_Player_name()
        {
            Player player = new Player("TestHero", 100);
            Assert.AreEqual("TestHero", player.Name);
        }
        [Test]
        public void Check_if_Player_Health_IsEqual()
        {
            var player = new Player("TestSpeler", 100);
            Assert.AreEqual(100, player.Health);
        }
        [Test]
        public void Taking_Damage_Ignores_Negative_Damage()
        {
            Player player = new Player("Hero",100);
            player.TakeDamage(-25);
            Assert.AreEqual(100, player.Health);
        }
        [Test]
        public void Player_takes_Damage()
        {
            Player player = new Player("TestHero", 100);

            player.TakeDamage(30);

            Assert.AreEqual(70, player.Health);
        }
        [Test]
        public void Player_Wins_game()
        {
            Player player = new Player("TestHero", 100);

            player.IsWinner = true;
            Assert.True(player.IsWinner);
        }
       
        [Test]
        public void Player_Have_not_won_Game()
        {
            var player = new Player("TestSpeler", 100);
            Assert.False(player.IsWinner);
        }
        
    }
}
