using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tests.Common;

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
                Stats = new TestTacticalActStatsSubScheme(),
                Dependency = new[] {
                    new TacticalActDependencySubScheme(default(SkillStatType), 1)
                }
            };



            // ACT
            var tacticalAct = new TacticalAct(tacticalActScheme);



            // ASSERT
            tacticalAct.Efficient.Dice.Should().Be(3);
            tacticalAct.Efficient.Count.Should().Be(1);
        }
    }
}