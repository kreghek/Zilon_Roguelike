using System;
using System.Threading.Tasks;

using Zilon.Core.MapGenerators.PrimitiveStyle;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.MapGenerators
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class SquareMapFactoryTests
    {
        /// <summary>
        /// Тест проверяет, что для карты создаётся корректный набор ребёр между узлами.
        /// </summary>
        [Test]
        public async Task Create_FixedMap_EdgesAreCorrectAsync()
        {
            // ARRANGE

            // ACT
            var map = await SquareMapFactory.CreateAsync(7).ConfigureAwait(false);

            // ASSERT
            AssertEdge(map, 0, 0, 1, 0);
            AssertEdge(map, 3, 1, 4, 2);

            // полный набор соседей
            AssertEdge(map, 1, 1, 0, 1);
            AssertEdge(map, 1, 1, 1, 0);
            AssertEdge(map, 1, 1, 2, 0);
            AssertEdge(map, 1, 1, 2, 1);
            AssertEdge(map, 1, 1, 2, 2);
            AssertEdge(map, 1, 1, 1, 2);

            // полный набор соседей для углового
            AssertEdge(map, 6, 6, 5, 6);
            AssertEdge(map, 6, 6, 5, 5);
            AssertEdge(map, 6, 6, 6, 5);
        }

        /// <summary>
        /// Тест проверяет, что генератор сеточных карт может работать с <see cref="HexMap"/>.
        /// <see cref="HexMap"/> используется на клиенте.
        /// </summary>
        [Test]
        public void Create_HexMapType_NoExceptions()
        {
            // ARRANGE

            // ACT
            Action act = () =>
            {
                var map = SquareMapFactory.CreateAsync(7);
            };

            // ASSERT
            act.Should().NotThrow();
        }

        private static bool HasEdge(IMap map, HexNode node, HexNode neighbor)
        {
            var neighbors = map.GetNext(node);
            return neighbors.Contains(neighbor);
        }

        private void AssertEdge(
            IMap map,
            int offsetX1,
            int offsetY1,
            int offsetX2,
            int offsetY2)
        {
            var node1 = map.Nodes.SelectByHexCoords(offsetX1, offsetY1);
            var node2 = map.Nodes.SelectByHexCoords(offsetX2, offsetY2);
            var hasEdge = HasEdge(map, node1, node2);
            hasEdge.Should().BeTrue();
        }
    }
}