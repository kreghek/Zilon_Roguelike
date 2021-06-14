using FluentAssertions;

using JetBrains.Annotations;

using TechTalk.SpecFlow;

using Zilon.Core.Common;
using Zilon.Core.Specs.Contexts;
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

        [Then(@"Монстр Id:(\d+) стоит в узле \((\d+), (\d+)\)")]
        [Then(@"Монстр Id:(\d+)\s(не)\sстоит в узле \((\d+), (\d+)\)")]
        public void ThenМонстрIdСтоитВУзле(int monsterId, string isNot, int offsetX, int offsetY)
        {
            var monster = Context.GetMonsterById(monsterId);
            var node = (HexNode)monster.Node;
            var cubeCoords = node.CubeCoords;
            var offsetCoords = HexHelper.ConvertToOffset(cubeCoords);

            if (string.IsNullOrWhiteSpace(isNot) || isNot != "не")
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