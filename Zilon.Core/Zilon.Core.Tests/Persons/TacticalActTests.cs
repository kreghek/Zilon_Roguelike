using FluentAssertions;

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
                    Efficient = new Roll(3, 1),
                },
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