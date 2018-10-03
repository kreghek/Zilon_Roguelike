using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Common;
using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Persons
{
    [TestFixture]
    public class TacticalActTests
    {
        /// <summary>
        /// Тест проверяет, что при создании тактического действия корректно рассчитывается эффективность.
        /// </summary>
        [Test]
        public void TacticalActTest()
        {
            //ARRAGE

            var tacticalActScheme = new TacticalActScheme
            {
                Stats = new TacticalActStatsSubScheme
                {
                    Efficient = new Range<float>(1, 2),
                },
                Dependency = new[] {
                    new TacticalActDependencySubScheme(default(SkillStatType), 1)
                }
            };



            // ACT
            var tacticalAct = new TacticalAct(1, tacticalActScheme);



            // ASSERT
            tacticalAct.MinEfficient.Should().Be(1);
            tacticalAct.MaxEfficient.Should().Be(2);
        }
    }
}