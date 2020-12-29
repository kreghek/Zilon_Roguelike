using System;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Logics
{
    public class UseProviantLogicState : LogicStateBase
    {
        public override IActorTask GetTask(IActor actor, ISectorTaskSourceContext context,
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

            if (strategyData is null)
            {
                throw new ArgumentNullException(nameof(strategyData));
            }

            var usePropTask = CreateTask(actor, context, strategyData);
            if (usePropTask != null)
            {
                Complete = true;
                return usePropTask;
            }

            Complete = true;
            return null;
        }

        protected override void ResetData()
        {
            // Has no inner state.
        }

        private static UsePropTask CreateTask(IActor actor, ISectorTaskSourceContext context,
            ILogicStrategyData strategyData)
        {
            if (strategyData.ResourceToReduceHazard is null)
            {
                throw new InvalidOperationException(
                    $"Assign {nameof(strategyData.ResourceToReduceHazard)} value in the triggers first.");
            }

            var taskContxt = new ActorTaskContext(context.Sector);
            var prop = strategyData.ResourceToReduceHazard;
            // When prop was used it no need to store anymore.
            strategyData.ResourceToReduceHazard = null;
            return new UsePropTask(actor, taskContxt, prop);
        }
    }
}