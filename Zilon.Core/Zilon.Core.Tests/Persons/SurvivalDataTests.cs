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
                    args.KeyPoint.Type == SurvivalStatKeyPointType.Lesser &&
                    args.KeyPoint.Value == 0);
            }
        }
    }
}