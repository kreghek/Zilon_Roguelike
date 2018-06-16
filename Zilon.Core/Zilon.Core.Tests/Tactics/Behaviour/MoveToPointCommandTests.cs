using NUnit.Framework;
using Zilon.Core.Tactics.Behaviour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zilon.Core.Tactics.Map;
using Zilon.Core.Persons;
using FluentAssertions;

namespace Zilon.Core.Tactics.Behaviour.Tests
{
    [TestFixture()]
    public class MoveToPointCommandTests
    {
        [Test()]
        public void ExecuteTest()
        {
            // ARRANGE
            var map = new CombatMap();
            map.Nodes = new List<MapNode>();

            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    map.Nodes.Add(new MapNode() { Coordinates = new Math.Vector2(i, j) });
                }
            }

            var startNode = map.Nodes.SingleOrDefault(n => n.Coordinates.X == 3 && n.Coordinates.Y == 3);
            var finishNode = map.Nodes.SingleOrDefault(n => n.Coordinates.X == 1 && n.Coordinates.Y == 5);

            var expectedPath = new[] {
                map.Nodes.SingleOrDefault(n => n.Coordinates.X == 2 && n.Coordinates.Y == 3),
                map.Nodes.SingleOrDefault(n => n.Coordinates.X == 2 && n.Coordinates.Y == 4),
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