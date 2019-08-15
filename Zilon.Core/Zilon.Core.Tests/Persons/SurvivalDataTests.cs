using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Persons;
using Zilon.Core.Tests.Common.Schemes;
using Zilon.Core.Tests.Persons.TestCases;

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
            const SurvivalStatType STAT_TYPE = SurvivalStatType.Satiety;

            const int MIN_STAT_VALUE = 0;
            const int MAX_STAT_VALUE = 1;
            const int START_STAT_VALUE = MAX_STAT_VALUE;

            const int LESSER_SURVIVAL_STAT_KEYPOINT = 0;
            const SurvivalStatHazardLevel LESSER_SURVIVAL_STAT_KEYPOINT_TYPE = SurvivalStatHazardLevel.Lesser;

            const int STAT_RATE = 1;

            const int EXPECTED_SURVIVAL_STAT_KEYPOINT = LESSER_SURVIVAL_STAT_KEYPOINT;

            // 1 при броске на снижение означает, что тест на снижение не пройден.
            // Потому что нужно 4+ по умолчанию, чтобы пройти.
            // Значит характеристика будет снижена.
            const int FAKE_ROLL_SURVIVAL_RESULT = 1;

            var survivalRandomSourceMock = new Mock<ISurvivalRandomSource>();
            survivalRandomSourceMock.Setup(x => x.RollSurvival(It.IsAny<SurvivalStat>()))
                .Returns(FAKE_ROLL_SURVIVAL_RESULT);
            var survivalRandomSource = survivalRandomSourceMock.Object;

            var survivalStats = new SurvivalStat[] {
                new SurvivalStat(START_STAT_VALUE, MIN_STAT_VALUE, MAX_STAT_VALUE){
                    Type = STAT_TYPE,
                    Rate = STAT_RATE,
                    KeyPoints = new[]{
                        new SurvivalStatKeyPoint(
                            LESSER_SURVIVAL_STAT_KEYPOINT_TYPE,
                            LESSER_SURVIVAL_STAT_KEYPOINT)
                    }
                }
            };

            var survivalData = new HumanSurvivalData(_personScheme,
                survivalStats,
                survivalRandomSource);



            // ACT
            using (var monitor = survivalData.Monitor())
            {
                survivalData.Update();



                // ASSERT
                monitor.Should().Raise(nameof(ISurvivalData.StatCrossKeyValue))
                    .WithArgs<SurvivalStatChangedEventArgs>(args =>
                    args.Stat.Type == STAT_TYPE &&
                    args.KeyPoints.FirstOrDefault().Level == LESSER_SURVIVAL_STAT_KEYPOINT_TYPE &&
                    args.KeyPoints.FirstOrDefault().Value == EXPECTED_SURVIVAL_STAT_KEYPOINT);
            }
        }

        /// <summary>
        /// Тест проверяет, что характеристика с изменённым DownPass корректно
        /// изменяется при указанных результатах броска кости.
        /// </summary>
        [Test]
        [TestCaseSource(typeof(SurvivalDataTestCasesSource), nameof(SurvivalDataTestCasesSource.DownPassTestCases))]
        public int Update_ModifiedDownPass_StatDownCorrectly(int statDownPass, int downPassRoll)
        {
            // ARRANGE

            const int STAT_RATE = 1;
            const int MIN_STAT_VALUE = 0;
            const int MAX_STAT_VALUE = 1;
            const int START_STAT_VALUE = MAX_STAT_VALUE;
            const SurvivalStatType STAT_TYPE = SurvivalStatType.Satiety;

            var survivalRandomSourceMock = new Mock<ISurvivalRandomSource>();
            survivalRandomSourceMock.Setup(x => x.RollSurvival(It.IsAny<SurvivalStat>()))
                .Returns(downPassRoll);
            var survivalRandomSource = survivalRandomSourceMock.Object;

            var survivalStats = new SurvivalStat[] {
                new SurvivalStat(START_STAT_VALUE, MIN_STAT_VALUE, MAX_STAT_VALUE){
                    Type = STAT_TYPE,
                    Rate = STAT_RATE,
                    DownPassRoll = statDownPass
                }
            };

            var survivalData = new HumanSurvivalData(_personScheme,
                survivalStats,
                survivalRandomSource);



            // ACT
            survivalData.Update();



            // ASSERT
            return survivalStats[0].Value;
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
                monitor.Should().Raise(nameof(HumanSurvivalData.Dead));
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
            var survivalData = new HumanSurvivalData(_personScheme, _survivalRandomSource);
            return survivalData;
        }
    }
}