using System.Collections.Generic;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Graphs;
using Zilon.Core.MapGenerators;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.MapGenerators
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class MapRegionHelperTests
    {
        /// <summary>
        /// Тест проверяет, что узел, который является единственным выходом, не выбирается.
        /// </summary>
        [Test]
        public void FindNonBlockedNode_RoomWithCorridor_ExitNotBlocked()
        {
            // ARRANGE
            var map = new GraphMap();

            // В этом тесте можно использовать более простые реализации IMapNode

            // генерируем комнату
            var node00 = new HexNode(0, 0);
            map.AddNode(node00);
            var node10 = new HexNode(1, 0);
            map.AddNode(node10);

            var node01 = new HexNode(0, 1);
            map.AddNode(node01);
            var node11 = new HexNode(1, 1);
            map.AddNode(node11);

            map.AddEdge(node00, node10);
            map.AddEdge(node00, node01);

            map.AddEdge(node11, node10);
            map.AddEdge(node11, node01);

            var regionNodes = new IGraphNode[] { node00, node01, node10, node11 };

            // генерируем выход
            var corridorNode = new HexNode(2, 0);
            map.AddNode(corridorNode);
            map.AddEdge(corridorNode, node10);

            // ACT
            var node = MapRegionHelper.FindNonBlockedNode(node10, map, regionNodes);

            // ASSERT
            node.Should().NotBe(node10);
            node.Should().NotBe(corridorNode);
        }

        /// <summary>
        /// Тест проверяет, что при поиске узла корректный узел находится, даже если первым начинается
        /// выбираться узел коридора.
        /// </summary>
        [Test]
        public void FindNonBlockedNode_NextIsCorridorNode_NodeFound()
        {
            // ARRANGE
            var mapMock = new Mock<GraphMap>().As<IMap>();
            mapMock.CallBase = true;
            var map = mapMock.Object;

            // В этом тесте можно использовать более простые реализации IMapNode

            // генерируем комнату
            var node00 = new HexNode(0, 0);
            map.AddNode(node00);
            var node10 = new HexNode(1, 0);
            map.AddNode(node10);

            var node01 = new HexNode(0, 1);
            map.AddNode(node01);
            var node11 = new HexNode(1, 1);
            map.AddNode(node11);

            map.AddEdge(node00, node10);
            map.AddEdge(node00, node01);

            map.AddEdge(node11, node10);
            map.AddEdge(node11, node01);

            var regionNodes = new IGraphNode[] { node00, node01, node10, node11 };

            // генерируем выход
            var corridorNode = new HexNode(2, 0);
            map.AddNode(corridorNode);
            map.AddEdge(corridorNode, node10);

            mapMock.Setup(x => x.GetNext(It.Is<IGraphNode>(n => n == node10)))
                .Returns(new IGraphNode[] { corridorNode, node00, node11 });

            // ACT
            var node = MapRegionHelper.FindNonBlockedNode(node10, map, regionNodes);

            // ASSERT
            node.Should().NotBe(node10);
            node.Should().NotBe(corridorNode);
        }

        /// <summary>
        /// Тест воспроизводит ошибку, возникующую в MoveCommandTest
        /// в коммите SHA-1: df2c306ddd744bcd32f9b8b47839838f9982fd85
        /// </summary>
        [Test]
        public void FindNonBlockedNode_80409354_NodeFound()
        {
            // ARRANGE
            var hexMap = new HexMap(200);

            var region = new List<IGraphNode>();
            for (var x = 80; x <= 93; x++)
            {
                for (var y = 40; y <= 54; y++)
                {
                    var node = new HexNode(x, y);
                    region.Add(node);
                    hexMap.AddNode(node);
                }
            }

            // генерация коридоров
            hexMap.AddNode(new HexNode(79, 40));
            hexMap.AddNode(new HexNode(78, 40));

            hexMap.AddNode(new HexNode(39, 89));
            hexMap.AddNode(new HexNode(38, 89));

            hexMap.AddNode(new HexNode(39, 90));
            hexMap.AddNode(new HexNode(38, 90));

            hexMap.AddNode(new HexNode(94, 40));
            hexMap.AddNode(new HexNode(95, 40));

            hexMap.AddNode(new HexNode(94, 48));
            hexMap.AddNode(new HexNode(94, 49));
            hexMap.AddNode(new HexNode(95, 49));
            hexMap.AddNode(new HexNode(95, 50));

            // эмулируем выборку сундуков в предыдущих итерациях
            var availableNodes = new List<IGraphNode>(region);
            var rolled = new[] { 114, 136, 0, 123, 179, 0, 111, 3 };
            foreach (int rolledIndex in rolled)
            {
                var rolledNode = availableNodes[rolledIndex]; // узел, который валил поиск.
                var selectedNode = MapRegionHelper.FindNonBlockedNode(rolledNode, hexMap, availableNodes);

                availableNodes.Remove(selectedNode);
            }

            // ACT
            var testedNode = availableNodes[0]; // узел, который валил поиск.
            var factNode = MapRegionHelper.FindNonBlockedNode(testedNode, hexMap, availableNodes);

            // ASSERT
            factNode.Should().NotBeNull();
        }
    }
}