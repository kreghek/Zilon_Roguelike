using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Common;
using Zilon.Core.Components;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons.Tests
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
                Efficient = new Range<float>(1, 2),
                Dependency = new[] {
                    new TacticalActDependencySubScheme(default(CombatStatType), 1)
                }
            };

            var combatStatsMock = new Mock<ICombatStats>();
            combatStatsMock.SetupGet(x => x.Stats)
                .Returns(new[] {
                    new CombatStatItem{ Stat = default(CombatStatType), Value = 10 }
                });
            var combatStats = combatStatsMock.Object;



            // ACT
            var tacticalAct = new TacticalAct(1, tacticalActScheme, combatStats);


            // ASSERT
            tacticalAct.MinEfficient.Should().Be(1);
            tacticalAct.MaxEfficient.Should().Be(2);
        }
    }
}