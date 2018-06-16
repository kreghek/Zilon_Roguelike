using NUnit.Framework;
using System.Linq;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Persons;
using FluentAssertions;
using Zilon.Core.Tests.TestCommon;

namespace Zilon.Core.Tactics.Behaviour.Tests
{
    [TestFixture()]
    public class MoveToPointCommandTests
    {
        [Test()]
        public void ExecuteTest()
        {
            // ARRANGE
            var map = new TestMap();

            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    map.Nodes.Add(new HexNode(i, j));
                }
            }

            var startNode = map.Nodes.SingleOrDefault(n => n.OffsetX == 3 && n.OffsetY == 3);
            var finishNode = map.Nodes.SingleOrDefault(n => n.OffsetX == 1 && n.OffsetY == 5);

            var expectedPath = new[] {
                map.Nodes.SingleOrDefault(n => n.OffsetX == 2 && n.OffsetY == 3),
                map.Nodes.SingleOrDefault(n => n.OffsetX == 2 && n.OffsetY == 4),
                finishNode
            };

            var actor = new Actor(new Person(), startNode);

            var moveCommand = new MoveToPointCommand(actor, finishNode, map);


            // ACT
            // ARRANGE
            for (var step = 1; step <= 3; step++)
            {
                moveCommand.Execute();

                if (step < 3)
                {
                    moveCommand.IsComplete.Should().Be(false);
                }
                else
                {
                    moveCommand.IsComplete.Should().Be(true);
                }

                actor.Node.Should().Be(expectedPath[step - 1]);
            }

            moveCommand.IsComplete.Should().Be(true);
            actor.Node.Should().Be(finishNode);
        }
    }
}