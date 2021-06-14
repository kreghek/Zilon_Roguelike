using System;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Triggers
{
    public abstract class HazardAndHasResourceTriggerBase : ILogicStateTrigger
    {
        protected abstract bool TestInner(
            IActor actor,
            ISectorTaskSourceContext context,
            ILogicState currentState,
            ILogicStrategyData strategyData);

        public void Reset()
        {
            // Has no state
        }

        public bool Test(
            IActor actor,
            ISectorTaskSourceContext context,
            ILogicState currentState,
            ILogicStrategyData strategyData)
        {
            if (actor is null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (currentState is null)
            {
                throw new ArgumentNullException(nameof(currentState));
            }

            if (strategyData is null)
            {
                throw new ArgumentNullException(nameof(strategyData));
            }

            return TestInner(actor, context, currentState, strategyData);
        }

        public void Update()
        {
            // Has no state.
        }
    }
}