using FluentAssertions;

using TechTalk.SpecFlow;

using Zilon.Core.Common;
using Zilon.Core.Spec.Contexts;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Spec.Steps
{
    [Binding]
    public sealed class MonsterSteps : GenericStepsBase<CommonGameActionsContext>
    {
        public MonsterSteps(CommonGameActionsContext context) : base(context)
        {
        }

        [Then(@"Монстр Id:(\d+) стоит в узле \((\d+), (\d+)\)")]
        public void ThenМонстрIdСтоитВУзле(int monsterId, int offsetX, int offsetY)
        {
            var monster = Context.GetMonsterById(monsterId);
            var node = (HexNode)monster.Node;
            var cubeCoords = node.CubeCoords;
            var offsetCoords = HexHelper.ConvertToOffset(cubeCoords);
            offsetCoords.Should().Be(new OffsetCoords(offsetX, offsetY));
        }
    }
}
