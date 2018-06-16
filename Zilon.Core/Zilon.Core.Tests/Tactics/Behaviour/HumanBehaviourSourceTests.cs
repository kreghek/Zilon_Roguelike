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
    public class HumanBehaviourSourceTests
    {
        [Test()]
        public void GetCommandsTest()
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

            var behSource = new HumanBehaviourSource(actor);
            behSource.AssignMoveToPointCommand(finishNode);


            // ACT
            // ARRANGE

            // 3 шага одна и та же команда, на 4 шаг - null-комманда
            for (var step = 1; step <= 4; step++)
            {
                var commands = behSource.GetCommands(map, new[] { actor });

                if (step < 4)
                {
                    commands.Count().Should().Be(1);

                    var factCommand = commands[0] as MoveToPointCommand;
                    factCommand.Should().NotBeNull();

                    factCommand.IsComplete.Should().Be(false);


                    foreach (var command in commands)
                    {
                        command.Execute();
                    }

                    actor.Node.Should().Be(expectedPath[step - 1]);
                }
                else
                {
                    commands.Should().BeNull();
                }
            }

            actor.Node.Should().Be(finishNode);
        }
    }
}