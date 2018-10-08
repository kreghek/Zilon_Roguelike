using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.Persons;

namespace Zilon.Core.Tests.Persons
{
    [TestFixture]
    public class SurvivalDataTests
    {
        /// <summary>
        /// This test is to show the value variable is being range checked.
        /// </summary>
        [Test]
        public void Update_TestStatValueItemSetter()
        {
            //ARRANGE
            var survivalData = new SurvivalData();
            var stat = survivalData.Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Satiety);
            //ACT
            stat.Value = 0;
            stat.Value.Should().Be(0);
            stat.Value = stat.Range.Min - 2;
            stat.Value.Should().Be(stat.Range.Min);
            stat.Value = stat.Range.Max + 2;
            stat.Value.Should().Be(stat.Range.Min);
        }

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
    }
}