using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zilon.Core.Common;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.World.Tests
{
    [TestFixture]
    public class ResourceMaterializationMapTests
    {
        [Test]
        [Category("development")]
        public async Task GetDepositDataTestAsync()
        {
            var sectorGeneratorMock = new Mock<ISectorGenerator>();
            sectorGeneratorMock.Setup(x => x.GenerateAsync(It.IsAny<ISectorNode>()))
                .Returns<ISectorNode>(scheme =>
                {
                    return Task.Run(() =>
                    {
                        var sectorMock = new Mock<ISector>();
                        var sector = sectorMock.Object;
                        return sector;
                    });
                });

            var sectorGenerator = sectorGeneratorMock.Object;
            ILocationScheme[] locationSchemes = CreateBiomSchemes();

            int schemeRoundCounter = 0;
            var rollerMock = new Mock<IBiomeSchemeRoller>();
            rollerMock.Setup(x => x.Roll()).Returns(() =>
            {
                schemeRoundCounter++;
                if (schemeRoundCounter >= locationSchemes.Length)
                {
                    schemeRoundCounter = 0;
                }

                ILocationScheme scheme = locationSchemes[schemeRoundCounter];

                return scheme;
            });
            var roller = rollerMock.Object;

            ILocationScheme introScheme = locationSchemes.Single(x => x.Sid == "intro");

            BiomeInitializer biomService = new BiomeInitializer(sectorGenerator, roller);

            LinearDice dice = new LinearDice();
            ResourceMaterializationMap resourceMaterializationMap = new ResourceMaterializationMap(dice);

            StringBuilder resultStringBuilder = new StringBuilder();
            resultStringBuilder.AppendLine("graph TD;");

            // ACT

            var biom = await biomService.InitBiomeAsync(introScheme).ConfigureAwait(false);
            var introNode = biom.Sectors.Single(x => x.State == SectorNodeState.SectorMaterialized);
            IResourceDepositData currentResource = resourceMaterializationMap.GetDepositData(introNode);
            ISectorNode currentNode = introNode;

            int iteration = 0;
            const int ITERATION_MAX = 100;

            List<NodeInfo> openList = new List<NodeInfo>();
            var nextNodes1 = currentNode.Biome.GetNext(currentNode)
                .OfType<SectorNode>()
                .Where(x => x.State != SectorNodeState.SectorMaterialized)
                .Select(x => new NodeInfo {Current = x, Parent = introNode, ParentResource = currentResource});
            openList.AddRange(nextNodes1);

            while (iteration < ITERATION_MAX)
            {
                NodeInfo nextNode = openList[0];
                openList.RemoveAt(0);

                await biomService.MaterializeLevelAsync(nextNode.Current).ConfigureAwait(false);
                IResourceDepositData nextResource = resourceMaterializationMap.GetDepositData(nextNode.Current);

                resultStringBuilder.AppendLine(GetVisualString(nextNode.Parent, nextNode.Current,
                    nextNode.ParentResource, nextResource));

                var nextNodes2 = nextNode.Current.Biome.GetNext(nextNode.Current)
                    .OfType<SectorNode>()
                    .Where(x => x.State != SectorNodeState.SectorMaterialized)
                    .Select(x => new NodeInfo {Current = x, Parent = nextNode.Current, ParentResource = nextResource});
                openList.AddRange(nextNodes2);

                iteration++;
            }

            Console.Write(resultStringBuilder.ToString());
        }

        private static string GetVisualString(ISectorNode currentNode, ISectorNode nextNode,
            IResourceDepositData currentResource, IResourceDepositData nextResource)
        {
            StringBuilder str =
                new StringBuilder(
                    $"    {currentNode.GetHashCode()}{ResourceToString(currentResource)}-->{nextNode.GetHashCode()}{ResourceToString(nextResource)};");

            if (currentResource.Items.Any())
            {
                Color currentResColor = ResourceToStyle(currentResource);
                str.AppendLine($"    style {currentNode.GetHashCode()} fill:{ColorTranslator.ToHtml(currentResColor)}");
            }

            if (nextResource.Items.Any())
            {
                Color nextResColor = ResourceToStyle(nextResource);
                str.AppendLine($"    style {nextNode.GetHashCode()} fill:{ColorTranslator.ToHtml(nextResColor)}");
            }

            return str.ToString();
        }

        private static Color ResourceToStyle(IResourceDepositData currentResource)
        {
            Color totalColor = new Color();
            Dictionary<SectorResourceType, Color> colorDict = new Dictionary<SectorResourceType, Color>
            {
                {SectorResourceType.Iron, Color.Maroon},
                {SectorResourceType.Stones, Color.Silver},
                {SectorResourceType.WaterPuddles, Color.Aqua},
                {SectorResourceType.CherryBrushes, Color.Green}
            };

            foreach (var item in currentResource.Items)
            {
                Color resColor = colorDict[item.ResourceType];
                float r = MathUtils.Lerp(totalColor.R, resColor.R, item.Share);
                float g = MathUtils.Lerp(totalColor.G, resColor.G, item.Share);
                float b = MathUtils.Lerp(totalColor.B, resColor.B, item.Share);

                totalColor = Color.FromArgb((int)r, (int)g, (int)b);
            }

            return totalColor;
        }

        private static string ResourceToString(IResourceDepositData currentResource)
        {
            if (currentResource.Items.Any())
            {
                Dictionary<SectorResourceType, string> colorDict = new Dictionary<SectorResourceType, string>
                {
                    {SectorResourceType.Iron, "i"},
                    {SectorResourceType.Stones, "s"},
                    {SectorResourceType.WaterPuddles, "w"},
                    {SectorResourceType.CherryBrushes, "b"}
                };

                StringBuilder sb = new StringBuilder();
                foreach (var item in currentResource.Items)
                {
                    string resColor = colorDict[item.ResourceType];

                    sb.Append($"{resColor}-{Math.Round(item.Share, 2).ToString(CultureInfo.InvariantCulture)} ");
                }

                return $"[{sb.ToString().Trim()}]";
            }

            return string.Empty;
        }

        private static ILocationScheme[] CreateBiomSchemes()
        {
            return new ILocationScheme[]
            {
                new TestLocationScheme
                {
                    Sid = "intro",
                    SectorLevels = new[]
                    {
                        new TestSectorSubScheme
                        {
                            Sid = "intro-1",
                            IsStart = true,
                            TransSectorSids = new[]
                            {
                                new TestSectorTransitionSubScheme {SectorLevelSid = "intro-2"}
                            }
                        },
                        new TestSectorSubScheme
                        {
                            Sid = "intro-2",
                            TransSectorSids = new[] {new TestSectorTransitionSubScheme()}
                        }
                    }
                },
                new TestLocationScheme
                {
                    Sid = "d1",
                    SectorLevels = new[]
                    {
                        new TestSectorSubScheme
                        {
                            Sid = "d1-1",
                            IsStart = true,
                            TransSectorSids = new[]
                            {
                                new TestSectorTransitionSubScheme {SectorLevelSid = "d1-2"}
                            }
                        },
                        new TestSectorSubScheme
                        {
                            Sid = "d1-2", TransSectorSids = new[] {new TestSectorTransitionSubScheme()}
                        }
                    }
                },
                new TestLocationScheme
                {
                    Sid = "d2",
                    SectorLevels = new[]
                    {
                        new TestSectorSubScheme
                        {
                            Sid = "d2-1",
                            IsStart = true,
                            TransSectorSids = new[] {new TestSectorTransitionSubScheme()}
                        }
                    }
                },
                new TestLocationScheme
                {
                    Sid = "d3",
                    SectorLevels = new[]
                    {
                        new TestSectorSubScheme
                        {
                            Sid = "d3-1",
                            IsStart = true,
                            TransSectorSids = new[]
                            {
                                new TestSectorTransitionSubScheme {SectorLevelSid = "d3-3"},
                                new TestSectorTransitionSubScheme {SectorLevelSid = "d3-4"}
                            }
                        },
                        new TestSectorSubScheme
                        {
                            Sid = "d3-3",
                            TransSectorSids = new[]
                            {
                                new TestSectorTransitionSubScheme {SectorLevelSid = "d3-2"}
                            }
                        },
                        new TestSectorSubScheme
                        {
                            Sid = "d3-4",
                            TransSectorSids = new[]
                            {
                                new TestSectorTransitionSubScheme {SectorLevelSid = "d3-3"}
                            }
                        },
                        new TestSectorSubScheme
                        {
                            Sid = "d3-2", TransSectorSids = new[] {new TestSectorTransitionSubScheme()}
                        }
                    }
                }
            };
        }

        private class NodeInfo
        {
            public ISectorNode Current;
            public ISectorNode Parent;
            public IResourceDepositData ParentResource;
        }
    }
}