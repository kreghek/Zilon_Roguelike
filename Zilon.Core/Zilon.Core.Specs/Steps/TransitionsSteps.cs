using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using TechTalk.SpecFlow;

using Zilon.Core.Commands;
using Zilon.Core.Players;
using Zilon.Core.Specs.Contexts;

namespace Zilon.Core.Specs.Steps
{
    [Binding]
    public sealed class TransitionsSteps : GenericStepsBase<CommonGameActionsContext>
    {
        public TransitionsSteps(CommonGameActionsContext context) : base(context)
        {
        }

        [Then(@"the player actor in the map with id:(\d+)")]
        public void ThenThePlayerActionInMapId(int expectedMapId)
        {
            var player = Context.ServiceProvider.GetRequiredService<IPlayer>();

            var sector = player.SectorNode.Sector;
            var map = sector.Map;

            map.Id.Should().Be(expectedMapId);
        }

        [When(@"I use current transition")]
        public void WhenIUseCurrentTransition()
        {
            var transitionCommand = Context.ServiceProvider.GetRequiredService<SectorTransitionMoveCommand>();
            transitionCommand.Execute();
        }
    }
}