using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using JetBrains.Annotations;

using TechTalk.SpecFlow;

using Zilon.Core.Common;
using Zilon.Core.Graphs;
using Zilon.Core.Specs.Contexts;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Specs.Steps
{
    [UsedImplicitly]
    [Binding]
    public sealed class MonsterSteps : GenericStepsBase<CommonGameActionsContext>
    {
        public MonsterSteps(CommonGameActionsContext context) : base(context)
        {
        }

        [Given(@"Для монстра Id:(\d+) задан маршрут")]
        public void GivenДляМонстраIdЗаданМаршрут(int monsterId, Table table)
        {
            var sector = Context.GetSector();

            var patrolPoints = new List<IGraphNode>();
            foreach (var tableRow in table.Rows)
            {
                tableRow.TryGetValue("x", out var routeX);
                tableRow.TryGetValue("y", out var routeY);

                var routeNode = sector.Map.Nodes.Cast<HexNode>()
                    .Single(node => node.OffsetX == int.Parse(routeX) && node.OffsetY == int.Parse(routeY));

                patrolPoints.Add(routeNode);
            }

            var route = new PatrolRoute(patrolPoints.ToArray());

            var monster = Context.GetMonsterById(monsterId);
            sector.PatrolRoutes[monster] = route;
        }


        [Then(@"Монстр Id:(\d+) стоит в узле \((\d+), (\d+)\)")]
        [Then(@"Монстр Id:(\d+)\s(не)\sстоит в узле \((\d+), (\d+)\)")]
        public void ThenМонстрIdСтоитВУзле(int monsterId, string isNot, int offsetX, int offsetY)
        {
            var monster = Context.GetMonsterById(monsterId);
            var node = (HexNode)monster.Node;
            var cubeCoords = node.CubeCoords;
            var offsetCoords = HexHelper.ConvertToOffset(cubeCoords);

            if (string.IsNullOrWhiteSpace(isNot))
            {
                offsetCoords.Should().Be(new OffsetCoords(offsetX, offsetY));
            }
            else
            {
                offsetCoords.Should().NotBe(new OffsetCoords(offsetX, offsetY));
            }
        }
    }
}
