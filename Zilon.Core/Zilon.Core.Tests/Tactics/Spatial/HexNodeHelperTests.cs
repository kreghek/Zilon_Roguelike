using System.Collections.Generic;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.Tactics.Spatial
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class HexNodeHelperTests
    {
        /// <summary>
        ///     Тест проверяет, что корректно выбираются пространственные соседи указанного узла.
        /// </summary>
        [Test]
        public void GetSpatialNeighborsTest()
        {
            // ARRANGE

            List<HexNode> nodes = new List<HexNode>();

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    nodes.Add(new HexNode(i, j));
                }
            }

            HexNode testedNode = nodes.SelectBy(3, 3);

            HexNode[] expectedNeighbors =
            {
                nodes.SelectBy(2, 3), nodes.SelectBy(3, 4), nodes.SelectBy(4, 4), nodes.SelectBy(4, 3),
                nodes.SelectBy(4, 2), nodes.SelectBy(3, 2)
            };

            // ACT
            HexNode[] factNeighbors = HexNodeHelper.GetSpatialNeighbors(testedNode, nodes.ToArray());

            // ASSERT
            for (int i = 0; i < 6; i++)
            {
                factNeighbors[i].OffsetCoords.Should().Be(expectedNeighbors[i].OffsetCoords);
            }

            factNeighbors.Should().BeEquivalentTo<HexNode>(expectedNeighbors);
        }
    }
}