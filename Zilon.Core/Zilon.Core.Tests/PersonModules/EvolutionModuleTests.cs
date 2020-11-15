using System;

using Zilon.Core.PersonModules;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.PersonModules
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class EvolutionModuleTests
    {
        /// <summary>
        /// Тест проверяет, что при получении следующего уровня перка, текущий уровень не сбрасывается.
        /// </summary>
        [Test]
        public void PerkLevelUpTest()
        {
            // ARRANGE
            var schemeServiceMock = new Mock<ISchemeService>();
            schemeServiceMock.Setup(x => x.GetSchemes<IPerkScheme>())
                .Returns(new IPerkScheme[]
                {
                    new PerkScheme
                    {
                        Levels = new[]
                        {
                            new PerkLevelSubScheme
                            {
                                MaxValue = 2, Jobs = Array.Empty<IJobSubScheme>()
                            }
                        }
                    }
                });
            var schemeService = schemeServiceMock.Object;

            var evolutionData = new EvolutionModule(schemeService);

            var perk = evolutionData.Perks[0];

            evolutionData.PerkLevelUp(perk);

            // ACT
            evolutionData.PerkLevelUp(perk);

            // ASSERT
            evolutionData.Perks.Length.Should().Be(1);
            evolutionData.Perks[0].CurrentLevel.Sub.Should().Be(1);
        }
    }
}