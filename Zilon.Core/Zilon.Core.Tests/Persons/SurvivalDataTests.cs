using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Persons;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.Persons
{
    [TestFixture]
    public class SurvivalDataTests
    {
        private TestPersonScheme _personScheme;
        private ISurvivalRandomSource _survivalRandomSource;

        /// <summary>
        /// Тест проверяет, что при достижении ключевого показателя модуль выживания генерирует событие.
        /// </summary>
        [Test]
        public void Update_StatNearKeyPoint_RaiseEventWithCorrectValues()
        {
            // ARRANGE
            var survivalRandomSourceMock = new Mock<ISurvivalRandomSource>();
            survivalRandomSourceMock.Setup(x => x.RollSurvival(It.IsAny<SurvivalStat>())).Returns(6);
            var survivalRandomSource = survivalRandomSourceMock.Object;

            ISurvivalData survivalData = SurvivalData.CreateHumanPersonSurvival(_personScheme, survivalRandomSource);

            var stat = survivalData.Stats.Single(x => x.Type == SurvivalStatType.Satiety);
            stat.Value = 1;



            // ACT
            using (var monitor = survivalData.Monitor())
            {
                survivalData.Update();



                // ASSERT
                monitor.Should().Raise(nameof(ISurvivalData.StatCrossKeyValue))
                    .WithArgs<SurvivalStatChangedEventArgs>(args =>
                    args.KeyPoints.FirstOrDefault().Level == SurvivalStatHazardLevel.Lesser &&
                    args.KeyPoints.FirstOrDefault().Value == 0);
            }
        }

        /// <summary>
        /// Тест проверяет, что при достижении ключевого показателя модуль выживания генерирует событие.
        /// </summary>
        [Test]
        public void RestoreStat_StatNearKeyPoint_RaiseEventWithCorrectValues()
        {
            // ARRANGE
            var survivalData = CreateSurvivalData();

            var stat = survivalData.Stats.Single(x => x.Type == SurvivalStatType.Satiety);
            stat.Value = -1;



            // ACT
            using (var monitor = survivalData.Monitor())
            {
                survivalData.RestoreStat(SurvivalStatType.Satiety, 1);



                // ASSERT
                monitor.Should().Raise(nameof(ISurvivalData.StatCrossKeyValue))
                    .WithArgs<SurvivalStatChangedEventArgs>(args =>
                    args.KeyPoints.FirstOrDefault().Level == SurvivalStatHazardLevel.Lesser &&
                    args.KeyPoints.FirstOrDefault().Value == 0);
            }
        }

        /// <summary>
        /// Тест проверяет, что при достижении ключевого показателя модуль выживания генерирует событие.
        /// </summary>
        [Test]
        public void RestoreStat_StatNearKeyPoint_RaiseEventWithCorrectValues2()
        {
            // ARRANGE
            var survivalData = CreateSurvivalData();

            var stat = survivalData.Stats.Single(x => x.Type == SurvivalStatType.Satiety);
            stat.Value = stat.KeyPoints[1].Value;
            var stat2 = survivalData.Stats.Single(x => x.Type == SurvivalStatType.Water);
            stat2.Value = stat2.KeyPoints[1].Value;

            // ACT
            using (var monitor = survivalData.Monitor())
            {
                survivalData.RestoreStat(SurvivalStatType.Water, 3);



                // ASSERT
                monitor.Should().Raise(nameof(ISurvivalData.StatCrossKeyValue))
                    .WithArgs<SurvivalStatChangedEventArgs>(args =>
                    args.KeyPoints.FirstOrDefault().Level == SurvivalStatHazardLevel.Strong &&
                    args.KeyPoints.FirstOrDefault().Value == stat2.KeyPoints[1].Value);
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

            _personScheme.Hp = personHp;

            var survivalData = CreateSurvivalData();

            var stat = survivalData.Stats.Single(x => x.Type == SurvivalStatType.Health);
            stat.Value = initialHp;



            // ACT
            survivalData.RestoreStat(SurvivalStatType.Health, restoreHpValue);



            // ASSERT
            var factStat = survivalData.Stats.Single(x => x.Type == SurvivalStatType.Health);
            factStat.Value.Should().Be(expectedHp);
        }

        /// <summary>
        /// Проверяет, что при потере всего здоровья выстреливает событие смерти.
        /// </summary>
        [Test]
        public void TakeDamage_FatalDamage_FiresEvent()
        {
            // ARRANGE

            const int personHp = 1;
            const int damageValue = 2;

            _personScheme.Hp = personHp;

            var survivalData = CreateSurvivalData();


            // ACT
            using (var monitor = survivalData.Monitor())
            {
                survivalData.DecreaseStat(SurvivalStatType.Health, damageValue);



                // ASSERT
                monitor.Should().Raise(nameof(SurvivalData.Dead));
            }
        }

        [SetUp]
        public void SetUp()
        {
            _personScheme = new TestPersonScheme();

            var survivalRandomSourceMock = new Mock<ISurvivalRandomSource>();
            _survivalRandomSource = survivalRandomSourceMock.Object;
        }


        private ISurvivalData CreateSurvivalData()
        {
            var survivalData = SurvivalData.CreateHumanPersonSurvival(_personScheme, _survivalRandomSource);
            return survivalData;
        }
    }
}