using System.Collections.Generic;
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
        ///     Тест проверяет, что узел, который является единственным выходом, не выбирается.
        /// </summary>
        [Test]
        public void FindNonBlockedNode_RoomWithCorridor_ExitNotBlocked()
        {
            // ARRANGE
            GraphMap map = new GraphMap();

            // В этом тесте можно использовать более простые реализации IMapNode

            // генерируем комнату
            HexNode node00 = new HexNode(0, 0);
            map.AddNode(node00);
            HexNode node10 = new HexNode(1, 0);
            map.AddNode(node10);

            HexNode node01 = new HexNode(0, 1);
            map.AddNode(node01);
            HexNode node11 = new HexNode(1, 1);
            map.AddNode(node11);

            map.AddEdge(node00, node10);
            map.AddEdge(node00, node01);

            map.AddEdge(node11, node10);
            map.AddEdge(node11, node01);

            IGraphNode[] regionNodes = {node00, node01, node10, node11};

            // генерируем выход
            HexNode corridorNode = new HexNode(2, 0);
            map.AddNode(corridorNode);
            map.AddEdge(corridorNode, node10);


            // ACT
            IGraphNode node = MapRegionHelper.FindNonBlockedNode(node10, map, regionNodes);


            // ASSERT
            node.Should().NotBe(node10);
            node.Should().NotBe(corridorNode);
        }

        /// <summary>
        ///     Тест проверяет, что при поиске узла корректный узел находится, даже если первым начинается
        ///     выбираться узел коридора.
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
            HexNode node00 = new HexNode(0, 0);
            map.AddNode(node00);
            HexNode node10 = new HexNode(1, 0);
            map.AddNode(node10);

            HexNode node01 = new HexNode(0, 1);
            map.AddNode(node01);
            HexNode node11 = new HexNode(1, 1);
            map.AddNode(node11);

            map.AddEdge(node00, node10);
            map.AddEdge(node00, node01);

            map.AddEdge(node11, node10);
            map.AddEdge(node11, node01);

            IGraphNode[] regionNodes = {node00, node01, node10, node11};

            // генерируем выход
            HexNode corridorNode = new HexNode(2, 0);
            map.AddNode(corridorNode);
            map.AddEdge(corridorNode, node10);

            mapMock.Setup(x => x.GetNext(It.Is<IGraphNode>(n => n == node10)))
                .Returns(new IGraphNode[] {corridorNode, node00, node11});


            // ACT
            IGraphNode node = MapRegionHelper.FindNonBlockedNode(node10, map, regionNodes);


            // ASSERT
            node.Should().NotBe(node10);
            node.Should().NotBe(corridorNode);
        }

        /// <summary>
        ///     Тест воспроизводит ошибку, возникующую в MoveCommandTest
        ///     в коммите SHA-1: df2c306ddd744bcd32f9b8b47839838f9982fd85
        /// </summary>
        [Test]
        public void FindNonBlockedNode_80409354_NodeFound()
        {
            // ARRANGE
            HexMap hexMap = new HexMap(200);

            List<IGraphNode> region = new List<IGraphNode>();
            for (int x = 80; x <= 93; x++)
            {
                for (int y = 40; y <= 54; y++)
                {
                    HexNode node = new HexNode(x, y);
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
            List<IGraphNode> availableNodes = new List<IGraphNode>(region);
            int[] rolled = {114, 136, 0, 123, 179, 0, 111, 3};
            foreach (int rolledIndex in rolled)
            {
                IGraphNode rolledNode = availableNodes[rolledIndex]; // узел, который валил поиск.
                IGraphNode selectedNode = MapRegionHelper.FindNonBlockedNode(rolledNode, hexMap, availableNodes);

                availableNodes.Remove(selectedNode);
            }


            // ACT
            IGraphNode testedNode = availableNodes[0]; // узел, который валил поиск.
            IGraphNode factNode = MapRegionHelper.FindNonBlockedNode(testedNode, hexMap, availableNodes);


            // ASSERT
            factNode.Should().NotBeNull();
        }
    }
}