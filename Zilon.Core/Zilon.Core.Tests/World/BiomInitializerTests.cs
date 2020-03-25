using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.MapGenerators;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.World.Tests
{
    [TestFixture()]
    [Category("integration")]
    public class BiomInitializerTests
    {
        [Test()]
        public async Task BuildLinearBiomGraph()
        {
            // ARRANGE

            var sectorGeneratorMock = new Mock<ISectorGenerator>();
            sectorGeneratorMock.Setup(x => x.GenerateAsync(It.IsAny<ISectorSubScheme>()))
                .Returns<ISectorSubScheme>(scheme =>
                {
                    return Task.Run(() =>
                    {
                        var sectorMock = new Mock<ISector>();
                        var sector = sectorMock.Object;
                        return sector;
                    });
                });

            var sectorGenerator = sectorGeneratorMock.Object;
            var locationSchemes = CreateBiomSchemes();

            var schemeRoundCounter = 0;
            var rollerMock = new Mock<IBiomeSchemeRoller>();
            rollerMock.Setup(x => x.Roll()).Returns(() =>
            {
                schemeRoundCounter++;
                if (schemeRoundCounter >= locationSchemes.Length)
                {
                    schemeRoundCounter = 0;
                }

                var scheme = locationSchemes[schemeRoundCounter];

                return scheme;
            });
            var roller = rollerMock.Object;

            var schemeServiceMock = new Mock<ISchemeService>();
            schemeServiceMock.Setup(x => x.GetSchemes<ILocationScheme>())
                .Returns(locationSchemes);
            schemeServiceMock.Setup(x => x.GetScheme<ILocationScheme>(It.IsAny<string>()))
                .Returns<string>(sid => locationSchemes.Single(scheme => scheme.Sid == sid));
            var schemeService = schemeServiceMock.Object;

            var introScheme = locationSchemes.Single(x => x.Sid == "intro");

            var biomService = new BiomeInitializer(sectorGenerator, roller);

            // ACT

            var biom = await biomService.InitBiomeAsync(introScheme).ConfigureAwait(false);
            var introNode = biom.Sectors.Single(x => x.State == SectorNodeState.SectorMaterialized);
            var currentNode = introNode;

            var iteration = 0;
            const int ITERATION_MAX = 100;
            while (iteration < ITERATION_MAX)
            {
                var nextNode = currentNode.Biome.GetNext(currentNode)
                    .OfType<SectorNode>()
                    .First(x => x.State != SectorNodeState.SectorMaterialized);

                await biomService.MaterializeLevel(nextNode).ConfigureAwait(false);

                currentNode = nextNode;

                iteration++;
            }

            // ASSERT
            var materializedSectorNodes = new List<SectorNode>();
            var scanNode = introNode;
            materializedSectorNodes.Add(scanNode);
            while (true)
            {
                var nextNodes = scanNode.Biome.GetNext(scanNode).OfType<SectorNode>();
                scanNode = nextNodes.SingleOrDefault(x => x.State == SectorNodeState.SectorMaterialized
                && !materializedSectorNodes.Contains(x));

                if (scanNode is null)
                {
                    break;
                }

                materializedSectorNodes.Add(scanNode);
            }

            var expectedMaterializedSectors = ITERATION_MAX + 1; // +1 потому что интровый не учитывается при итерациях.
            materializedSectorNodes.Should().HaveCount(expectedMaterializedSectors);
        }

        private static ILocationScheme[] CreateBiomSchemes()
        {
            return new ILocationScheme[] {
                new TestLocationScheme{
                    Sid = "intro",
                    SectorLevels = new []{
                        new TestSectorSubScheme{
                            Sid = "intro-1",
                            IsStart = true,
                            TransSectorSids = new[]{ new TestSectorTransitionSubScheme { SectorLevelSid = "intro-2" } }
                        },
                        new TestSectorSubScheme{
                            Sid = "intro-2",
                            TransSectorSids = new[]{ new TestSectorTransitionSubScheme() }
                        }
                    }
                },

                new TestLocationScheme{
                    Sid = "d1",
                    SectorLevels = new []{
                        new TestSectorSubScheme{
                            Sid = "d1-1",
                            IsStart = true,
                            TransSectorSids = new[]{ new TestSectorTransitionSubScheme { SectorLevelSid = "d1-2" } }
                        },
                        new TestSectorSubScheme{
                            Sid = "d1-2",
                            TransSectorSids = new[]{ new TestSectorTransitionSubScheme() }
                        }
                    }
                },

                new TestLocationScheme{
                    Sid = "d2",
                    SectorLevels = new []{
                        new TestSectorSubScheme{
                            Sid = "d2-1",
                            IsStart = true,
                            TransSectorSids = new[]{ new TestSectorTransitionSubScheme() }
                        }
                    }
                },

                new TestLocationScheme{
                    Sid = "d3",
                    SectorLevels = new []{
                        new TestSectorSubScheme{
                            Sid = "d3-1",
                            IsStart = true,
                            TransSectorSids = new[]{ new TestSectorTransitionSubScheme { SectorLevelSid = "d3-3" },
                                new TestSectorTransitionSubScheme { SectorLevelSid ="d3-4" } }
                        },

                        new TestSectorSubScheme{
                            Sid = "d3-3",
                            TransSectorSids = new[]{ new TestSectorTransitionSubScheme { SectorLevelSid = "d3-2" } }
                        },

                        new TestSectorSubScheme{
                            Sid = "d3-4",
                            TransSectorSids = new[]{ new TestSectorTransitionSubScheme { SectorLevelSid = "d3-3" } }
                        },

                        new TestSectorSubScheme{
                            Sid = "d3-2",
                            TransSectorSids = new[]{ new TestSectorTransitionSubScheme() }
                        }
                    }
                }
            };
        }
    }
}