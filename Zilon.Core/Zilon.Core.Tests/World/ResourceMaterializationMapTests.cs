using NUnit.Framework;
using Zilon.Core.World;
using System;
using System.Collections.Generic;
using System.Text;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
using Zilon.Core.Schemes;
using Moq;
using System.Threading.Tasks;
using Zilon.Core.Tests.Common.Schemes;
using System.Linq;
using Zilon.Core.Tactics;
using System.Drawing;
using Zilon.Core.Common;

namespace Zilon.Core.World.Tests
{
    [TestFixture]
    public class ResourceMaterializationMapTests
    {
        [Test]
        [Category("development")]
        public async System.Threading.Tasks.Task GetDepositDataTestAsync()
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

            var introScheme = locationSchemes.Single(x => x.Sid == "intro");

            var biomService = new BiomeInitializer(sectorGenerator, roller);

            var dice = new LinearDice();
            var resourceMaterializationMap = new ResourceMaterializationMap(dice);

            var resultStringBuilder = new StringBuilder();
            resultStringBuilder.AppendLine("graph TD;");

            // ACT

            var biom = await biomService.InitBiomeAsync(introScheme).ConfigureAwait(false);
            var introNode = biom.Sectors.Single(x => x.State == SectorNodeState.SectorMaterialized);
            var currentResource = resourceMaterializationMap.GetDepositData(introNode);
            var currentNode = introNode;

            var iteration = 0;
            const int ITERATION_MAX = 100;
            while (iteration < ITERATION_MAX)
            {
                var nextNode = currentNode.Biome.GetNext(currentNode)
                    .OfType<SectorNode>()
                    .First(x => x.State != SectorNodeState.SectorMaterialized);

                await biomService.MaterializeLevelAsync(nextNode).ConfigureAwait(false);
                var nextResource = resourceMaterializationMap.GetDepositData(nextNode);

                resultStringBuilder.AppendLine(GetVisualString(currentNode, nextNode, currentResource, nextResource));

                currentNode = nextNode;
                currentResource = nextResource;

                iteration++;
            }

            var materializedSectorNodes = new List<SectorNode>();
            var scanNode = introNode;
            materializedSectorNodes.Add(scanNode);
            while (true)
            {
                var nextNodes = scanNode.Biome.GetNext(scanNode).OfType<SectorNode>();
                scanNode = nextNodes.SingleOrDefault(x => x.State == SectorNodeState.SectorMaterialized && !materializedSectorNodes.Contains(x));

                if (scanNode is null)
                {
                    break;
                }

                materializedSectorNodes.Add(scanNode);
            }

            Console.Write(resultStringBuilder.ToString());
        }

        private static string GetVisualString(SectorNode currentNode, SectorNode nextNode, IResourceDepositData currentResource, IResourceDepositData nextResource)
        {
            var str = $"    {currentNode.GetHashCode()}[{ResourceToString(currentResource)}]-->{nextNode.GetHashCode()}[{ResourceToString(nextResource)}];";

            var currentResColor = ResourceToStyle(currentResource);
            var nextResColor = ResourceToStyle(nextResource);

            str += $"\n    style {currentNode.GetHashCode()} fill:{System.Drawing.ColorTranslator.ToHtml(currentResColor)}";
            str += $"\n    style {nextNode.GetHashCode()} fill:{System.Drawing.ColorTranslator.ToHtml(nextResColor)}";

            return str;
        }

        private static Color ResourceToStyle(IResourceDepositData currentResource)
        {
            var totalColor = new Color();
            var colorDict = new Dictionary<SectorResourceType, Color> {
                { SectorResourceType.Iron, Color.Maroon },
                { SectorResourceType.Stones, Color.Silver },
                { SectorResourceType.WaterPuddles, Color.Aqua },
                { SectorResourceType.CherryBrushes, Color.Green }
            };

            foreach (var item in currentResource.Items)
            {
                var resColor = colorDict[item.ResourceType];
                var r = MathUtils.Lerp(totalColor.R, resColor.R, item.Share);
                var g = MathUtils.Lerp(totalColor.G, resColor.G, item.Share);
                var b = MathUtils.Lerp(totalColor.B, resColor.B, item.Share);

                totalColor = Color.FromArgb((int)r, (int)g, (int)b);
            }

            return totalColor;
        }

        private static string ResourceToString(IResourceDepositData currentResource)
        {
            var colorDict = new Dictionary<SectorResourceType, string> {
                { SectorResourceType.Iron, "i" },
                { SectorResourceType.Stones, "s" },
                { SectorResourceType.WaterPuddles, "w" },
                { SectorResourceType.CherryBrushes, "b" }
            };

            var s = string.Empty;
            foreach (var item in currentResource.Items)
            {
                var resColor = colorDict[item.ResourceType];

                s += $"{resColor}-{Math.Round(item.Share, 2)} ";
            }

            return s;
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