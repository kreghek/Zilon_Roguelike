﻿using System.Threading.Tasks;

using FluentAssertions;

using JetBrains.Annotations;

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

        [UsedImplicitly]
        [Given(@"the linear globe")]
        public async Task GivenLinearGlobeAsync()
        {
            await Context.CreateLinearGlobeAsync().ConfigureAwait(false);
        }

        [UsedImplicitly]
        [Then(@"the player actor in the map with id:(\d+)")]
        public void ThenThePlayerActionInMapId(int expectedMapId)
        {
            var player = Context.ServiceProvider.GetRequiredService<IPlayer>();

            var sector = player.SectorNode.Sector;
            var map = sector.Map;

            map.Id.Should().Be(expectedMapId);
        }

        [When(@"the player person uses current transition")]
        public void WhenThePlayerPersonUsesCurrentTransition()
        {
            var transitCommand = Context.ServiceProvider.GetRequiredService<SectorTransitionMoveCommand>();
            transitCommand.Execute();
        }
    }
}