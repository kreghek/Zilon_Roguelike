using Microsoft.Extensions.DependencyInjection;

using TechTalk.SpecFlow;

using Zilon.Core.Commands;
using Zilon.Core.Specs.Contexts;

namespace Zilon.Core.Specs.Steps
{
    [Binding]
    public sealed class TransitionsSteps : GenericStepsBase<CommonGameActionsContext>
    {
        public TransitionsSteps(CommonGameActionsContext context) : base(context)
        {
        }

        [When(@"I use current transition")]
        public void WhenIUseCurrentTransition()
        {
            var transitionCommand = Context.ServiceProvider.GetRequiredService<SectorTransitionMoveCommand>();
            transitionCommand.Execute();
        }

    }
}
