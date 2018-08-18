using NUnit.Framework;
using Zilon.Core.Schemes;
using Moq;
using FluentAssertions;

namespace Zilon.Core.Persons.Tests
{
    [TestFixture()]
    public class EvolutionDataTests
    {
        /// <summary>
        /// Тест проверяет, что при получении следующего уровня перка, текущий уровень не сбрасывается.
        /// </summary>
        [Test()]
        public void PerkLevelUpTest()
        {
            // ARRANGE
            var schemeServiceMock = new Mock<ISchemeService>();
            schemeServiceMock.Setup(x => x.GetSchemes<PerkScheme>())
                .Returns(new PerkScheme[] {
                    new PerkScheme{
                        Levels = new[]{
                            new PerkLevelSubScheme{
                                MaxValue = 2,
                                Jobs = new JobSubScheme[0]
                            }
                        }
                    }
                });
            var schemeService = schemeServiceMock.Object;

            var evolutionData = new EvolutionData(schemeService);

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