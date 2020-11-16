using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Graphs;
using Zilon.Core.MapGenerators;
using Zilon.Core.PathFinding;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Tests.Tactics.Spatial.PathFinding
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class AStarSectorGraphMapTests
    {
        public static void AddWall(
            ISectorMap map,
            int x1,
            int y1,
            int x2,
            int y2)
        {
            map.RemoveEdge(x1, y1, x2, y2);
        }

        /// <summary>
        /// Тест из спеки:
        /// Перемещение актёра по узла каждый ход. На карте есть монстры и источник команд для них.
        /// Даёт уверенность, что в изоляции поиск пути выполняется. Была ошибка, что не находил путь.
        /// </summary>
        [Test]
        public async Task Run_FromSpec()
        {
            // ARRANGE

            // Шаг "Есть карта размером"

            const int mapSize = 3;

            ISectorMap map = new SectorGraphMap<HexNode, HexMapNodeDistanceCalculator>();

            MapFiller.FillSquareMap(map, mapSize);

            var mapRegion = new MapRegion(1, map.Nodes.ToArray())
            {
                IsStart = true,
                IsOut = true,
                ExitNodes = new[]
                {
                    map.Nodes.Last()
                }
            };

            map.Regions.Add(mapRegion);

            // Шаг "Между ячейками () и () есть стена"
            AddWall(map, 0, 0, 1, 0);
            AddWall(map, 1, 0, 0, 1);

            var path = new List<IGraphNode>();

            var startNode = map.Nodes.SelectByHexCoords(0, 0);
            var finishNode = map.Nodes.SelectByHexCoords(1, 0);

            var contextMock = new Mock<IAstarContext>();
            contextMock.Setup(x => x.GetNext(It.IsAny<IGraphNode>()))
                       .Returns<IGraphNode>(x => map.GetNext(x));
            contextMock.Setup(x => x.GetDistanceBetween(It.IsAny<IGraphNode>(), It.IsAny<IGraphNode>()))
                       .Returns<IGraphNode, IGraphNode>((a, b) => map.DistanceBetween(a, b));
            var context = contextMock.Object;

            // ACT

            map.FindPath(startNode, finishNode, context, path);

            // ASSERT

            path.Should()
                .NotBeEmpty();
        }
    }
}