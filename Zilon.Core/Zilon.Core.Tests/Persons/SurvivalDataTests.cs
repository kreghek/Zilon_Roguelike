using System.Linq;

using FluentAssertions;
using Moq;
using NUnit.Framework;

using Zilon.Core.Persons;

namespace Zilon.Core.Tests.Persons
{
    [TestFixture]
    public class SurvivalDataTests
    {
        /// <summary>
        /// Тест проверяет, что при достижении ключевого показателя модуль выживания генерирует событие.
        /// </summary>
        [Test]
        public void Update_StatNearKeyPoint_RaiseEventWithCorrentValues()
        {
            // ARRANGE
            var survivalData = new SurvivalData();

            var stat = survivalData.Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Satiety);
            stat.Value = 1;



            // ACT
            using (var monitor = survivalData.Monitor())
            {
                survivalData.Update();



                // ASSERT
                monitor.Should().Raise(nameof(ISurvivalData.StatCrossKeyValue))
                    .WithArgs<SurvivalStatChangedEventArgs>(args =>
                    args.KeyPoint.Level == SurvivalStatHazardLevel.Lesser &&
                    args.KeyPoint.Value == 0);
            }
        }

        /// <summary>
        /// Тест проверяет, что при достижении ключевого показателя модуль выживания генерирует событие.
        /// </summary>
        [Test]
        public void RestoreStat_StatNearKeyPoint_RaiseEventWithCorrentValues()
        {
            // ARRANGE
            var survivalData = new SurvivalData();

            var stat = survivalData.Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Satiety);
            stat.Value = -1;



            // ACT
            using (var monitor = survivalData.Monitor())
            {
                survivalData.RestoreStat(SurvivalStatType.Satiety, 1);



                // ASSERT
                monitor.Should().Raise(nameof(ISurvivalData.StatCrossKeyValue))
                    .WithArgs<SurvivalStatChangedEventArgs>(args =>
                    args.KeyPoint.Level == SurvivalStatHazardLevel.Lesser &&
                    args.KeyPoint.Value == 0);
            }
        }

        /// <summary>
        /// Тест проверяет, что при достижении ключевого показателя модуль выживания генерирует событие.
        /// </summary>
        [Test]
        public void RestoreStat_StatNearKeyPoint_RaiseEventWithCorrentValues2()
        {
            // ARRANGE
            var survivalData = new SurvivalData();

            var stat = survivalData.Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Satiety);
            stat.Value = -25;
            var stat2 = survivalData.Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Water);
            stat2.Value = -25;

            // ACT
            using (var monitor = survivalData.Monitor())
            {
                survivalData.RestoreStat(SurvivalStatType.Water, 3);



                // ASSERT
                monitor.Should().Raise(nameof(ISurvivalData.StatCrossKeyValue))
                    .WithArgs<SurvivalStatChangedEventArgs>(args =>
                    args.KeyPoint.Level == SurvivalStatHazardLevel.Strong &&
                    args.KeyPoint.Value == -25);
            }
        }

        /// <summary>
        /// Тест проверяет, что при восстановлении Хп текущее значение не выходит за рамки максимального.
        /// </summary>
        [Test]
        public void RestoreHp_RestoreHp_HpNotGreaterThatMaxPersonHp()
        {
            // ARRANGE
            const int initialHp = 2;
            const int personHp = 3;
            const int restoreHpValue = 2;
            const int expectedHp = personHp;

            var personMock = new Mock<IPerson>();
            personMock.SetupGet(x => x.Hp).Returns(personHp);
            var person = personMock.Object;

            var actorState = new SurvivalData();



            // ACT
            actorState.RestoreHp(restoreHpValue);



            // ASSERT
            actorState.Hp.Should().Be(expectedHp);
        }

        /// <summary>
        /// Проверяет, что актёр при потере всего здоровья выстреливает событие смерти.
        /// </summary>
        [Test]
        public void TakeDamage_FatalDamage_FiresEvent()
        {
            // ARRANGE

            var personMock = new Mock<IPerson>();
            personMock.SetupGet(x => x.Hp).Returns(1);
            var person = personMock.Object;

            var playerMock = new Mock<IPlayer>();
            var player = playerMock.Object;

            var nodeMock = new Mock<IMapNode>();
            var node = nodeMock.Object;

            var testActor = new Actor(person, player, node);


            // ACT
            using (var monitor = testActor.State.Monitor())
            {
                testActor.TakeDamage(1);



                // ASSERT
                monitor.Should().Raise(nameof(testActor.State.Dead));
            }
        }
    }
}