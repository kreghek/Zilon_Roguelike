using System.Collections.Generic;
using Zilon.Core.Common;
using Zilon.Core.Graphs;
using Zilon.Core.Specs.Contexts;
using Zilon.Core.Tactics;
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
            ISector sector = Context.GetCurrentGlobeFirstSector();

            List<IGraphNode> patrolPoints = new List<IGraphNode>();
            foreach (var tableRow in table.Rows)
            {
                tableRow.TryGetValue("x", out var routeX);
                tableRow.TryGetValue("y", out var routeY);

                var routeNode = sector.Map.Nodes.Cast<HexNode>()
                    .Single(node =>
                        node.OffsetCoords.X == int.Parse(routeX) && node.OffsetCoords.Y == int.Parse(routeY));

                patrolPoints.Add(routeNode);
            }

            PatrolRoute route = new PatrolRoute(patrolPoints.ToArray());

            IActor monster = Context.GetMonsterById(monsterId);
            sector.PatrolRoutes[monster] = route;
        }

        [Then(@"Монстр Id:(\d+) стоит в узле \((\d+), (\d+)\)")]
        [Then(@"Монстр Id:(\d+)\s(не)\sстоит в узле \((\d+), (\d+)\)")]
        public void ThenМонстрIdСтоитВУзле(int monsterId, string isNot, int offsetX, int offsetY)
        {
            IActor monster = Context.GetMonsterById(monsterId);
            HexNode node = (HexNode)monster.Node;
            CubeCoords cubeCoords = node.CubeCoords;
            OffsetCoords offsetCoords = HexHelper.ConvertToOffset(cubeCoords);

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