using System;

using Zilon.Bot.Players.Logics;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Triggers
{
    public class IntruderDetectedTrigger : ILogicStateTrigger
    {
        public bool Test(IActor actor, ISectorTaskSourceContext context, ILogicState currentState,
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

            var map = context.Sector.Map;
            var actorManager = context.Sector.ActorManager;

            var nearbyIntruder = IntruderDetectionHelper.GetIntruder(actor, map, actorManager);
            // Remember last intruder for logic with will handle reaction.
            strategyData.TriggerIntuder = nearbyIntruder;

            if (nearbyIntruder is null)
            {
                return false;
            }

            return true;
        }

        public void Update()
        {

        }

        public void Reset()
        {
            // Нет состояния.
        }
    }
}