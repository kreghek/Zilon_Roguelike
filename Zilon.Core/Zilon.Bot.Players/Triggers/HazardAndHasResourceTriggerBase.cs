
using JetBrains.Annotations;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Triggers
{
    public abstract class HazardAndHasResourceTriggerBase : ILogicStateTrigger
    {
        public void Reset()
        {
            // Has no state
            // TODO Reset strategyData.ResourceToReduceHazard
        }

        public bool Test(
            IActor actor,
            ISectorTaskSourceContext context,
            ILogicState currentState,
            ILogicStrategyData strategyData)
        {
            if (actor is null)
            {
                throw new System.ArgumentNullException(nameof(actor));
            }

            if (context is null)
            {
                throw new System.ArgumentNullException(nameof(context));
            }

            if (currentState is null)
            {
                throw new System.ArgumentNullException(nameof(currentState));
            }

            if (strategyData is null)
            {
                throw new System.ArgumentNullException(nameof(strategyData));
            }

            return TestInner(actor, context, currentState, strategyData);
        }

        protected abstract bool TestInner(
            [NotNull] IActor actor,
            [NotNull] ISectorTaskSourceContext context,
            [NotNull] ILogicState currentState,
            [NotNull] ILogicStrategyData strategyData);

        public void Update()
        {
            // Has no state.
        }
    }
}