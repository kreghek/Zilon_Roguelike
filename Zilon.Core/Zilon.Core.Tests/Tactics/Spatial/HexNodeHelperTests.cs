using System.Collections.Generic;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.Tests.Common;

namespace Zilon.Core.Tactics.Spatial.Tests
{
    [TestFixture()]
    public class HexNodeHelperTests
    {
        [Test()]
        public void GetNeighborsTest()
        {
            // ARRANGE

            var nodes = new List<HexNode>();

            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    nodes.Add(new HexNode(i, j));
                }
            }

            var testedNode = nodes.SelectBy(3, 3);

            var expectedNeibours = new[] {
                nodes.SelectBy(2, 3),

                nodes.SelectBy(3, 4),
                nodes.SelectBy(4, 4),

                nodes.SelectBy(4, 3),

                nodes.SelectBy(4, 2),
                nodes.SelectBy(3, 2)
            };


            // ACT
            var factNeibours = HexNodeHelper.GetNeighbors(testedNode, nodes.ToArray());



            // ASSERT
            for (var i = 0; i < 6; i++)
            {
                factNeibours[i].OffsetX.Should().Be(expectedNeibours[i].OffsetX);
                factNeibours[i].OffsetY.Should().Be(expectedNeibours[i].OffsetY);
            }

            factNeibours.Should().BeEquivalentTo(expectedNeibours);
        }
    }
}