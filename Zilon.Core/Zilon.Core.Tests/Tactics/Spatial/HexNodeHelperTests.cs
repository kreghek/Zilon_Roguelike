using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using NUnit.Framework;

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

            var testedNode = nodes.SingleOrDefault(n => n.OffsetX == 3 && n.OffsetY == 3);

            var expectedNeibours = new[] {
                nodes.SingleOrDefault(n => n.OffsetX == 3 && n.OffsetY == 2),
                nodes.SingleOrDefault(n => n.OffsetX == 4 && n.OffsetY == 2),

                nodes.SingleOrDefault(n => n.OffsetX == 2 && n.OffsetY == 3),
                nodes.SingleOrDefault(n => n.OffsetX == 4 && n.OffsetY == 3),

                nodes.SingleOrDefault(n => n.OffsetX == 3 && n.OffsetY == 4),
                nodes.SingleOrDefault(n => n.OffsetX == 4 && n.OffsetY == 4)
            };


            // ACT
            var factNeibours = HexNodeHelper.GetNeighbors(testedNode, nodes);



            // ASSERT
            factNeibours.Should().BeEquivalentTo(expectedNeibours);
        }
    }
}