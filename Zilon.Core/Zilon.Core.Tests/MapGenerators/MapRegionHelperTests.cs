using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.MapGenerators;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.MapGenerators
{
    [TestFixture]
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

            var regionNodes = new IMapNode[] { node00, node01, node10, node11 };

            // генерируем выход
            var corridorNode = new HexNode(2, 0);
            map.AddNode(corridorNode);
            map.AddEdge(corridorNode, node10);



            // ACT
            var node = MapRegionHelper.FindNonBlockedNode(map, regionNodes);


            // ASSERT
            node.Should().NotBe(node10);
        }
    }
}