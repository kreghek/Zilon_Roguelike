using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Persons
{
    [TestFixture]
    public class EvolutionDataTests
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
                .Returns(new IPerkScheme[] {
                    new PerkScheme{
                        Levels = new[]{
                            new PerkLevelSubScheme{
                                MaxValue = 2,
                                Jobs = new IJobSubScheme[0]
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