using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Schemes;
using Zilon.Core.World;

namespace Zilon.Core.Tests.World
{
    [TestFixture]
    public class GlobeHelperTests
    {
        /// <summary>
        /// Тест проверяет, что метод возвращает координатный центр из набора узлов.
        /// </summary>
        [Test]
        [TestCaseSource(typeof(GlobeHelperTestCaseSource), nameof(GlobeHelperTestCaseSource.TestCases))]
        public void GetCenterLocationNodeTest(int gridSize, int expectedCenterX, int expectedCenterY)
        {
            // ARRANGE

            var locationSchemeMock = new Mock<ILocationScheme>();
            var locationScheme = locationSchemeMock.Object;

            var nodes = new GlobeRegionNode[gridSize * gridSize];

            for (var i = 0; i < gridSize; i++)
            {
                for (var j = 0; j < gridSize; j++)
                {
                    nodes[i * gridSize + j] = new GlobeRegionNode(i, j, locationScheme);
                }
            }



            // ACT
            var factCenter = GlobeHelper.GetCenterLocationNode(nodes);



            // ASSERT
            var expectedCenter = nodes.Single(node => node.OffsetX == expectedCenterX && node.OffsetY == expectedCenterY);

            factCenter.Should().Be(expectedCenter);
        }
    }
}