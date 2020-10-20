using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Persons;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players.Triggers
{
    public class IntruderDetectedTrigger : ILogicStateTrigger
    {
        private static IActor[] CheckForIntruders(IActor actor, ISectorMap map, IActorManager actorManager)
        {
            var foundIntruders = new List<IActor>();

            foreach (var target in actorManager.Items)
            {
                if (target.Person.Fraction == actor.Person.Fraction)
                {
                    continue;
                }

                if (target.Person.CheckIsDead())
                {
                    continue;
                }

                var isVisible = LogicHelper.CheckTargetVisible(map, actor.Node, target.Node);
                if (!isVisible)
                {
                    continue;
                }

                foundIntruders.Add(target);
            }

            return foundIntruders.ToArray();
        }

        public bool Test(IActor actor, ISectorTaskSourceContext context, ILogicState currentState, ILogicStrategyData strategyData)
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

            var map = context.Sector.Map;
            var actorManager = context.Sector.ActorManager;

            // На каждом шаге осматриваем окрестности
            // на предмет нарушителей.
            var intruders = CheckForIntruders(actor, map, actorManager);

            var orderedIntruders = intruders.OrderBy(x => map.DistanceBetween(actor.Node, x.Node));
            var nearbyIntruder = orderedIntruders.FirstOrDefault();

            if (nearbyIntruder == null)
            {
                return false;
            }

            return true;
        }

        public void Update()
        {
            // Нет состояния.
        }

        public void Reset()
        {
            // Нет состояния.
        }
    }
}