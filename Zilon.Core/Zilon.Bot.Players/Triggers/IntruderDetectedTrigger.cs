using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players.Triggers
{
    public class IntruderDetectedTrigger: ILogicStateTrigger
    {
        private const int VISIBLE_RANGE = 5;
        private readonly IActorManager _actorManager;
        private readonly ISectorMap _map;

        public IntruderDetectedTrigger(IActorManager actorManager, ISectorMap map)
        {
            _actorManager = actorManager;
            _map = map;
        }

        private IActor[] CheckForIntruders(IActor actor)
        {
            var foundIntruders = new List<IActor>();
            foreach (var target in _actorManager.Items)
            {
                if (target.Owner == actor.Owner)
                {
                    continue;
                }

                if (target.Person.Survival.IsDead)
                {
                    continue;
                }

                var isVisible = CheckTargetVisible(actor.Node, target.Node);
                if (!isVisible)
                {
                    continue;
                }

                foundIntruders.Add(target);
            }

            return foundIntruders.ToArray();
        }

        private bool CheckTargetVisible(IMapNode node, IMapNode target)
        {
            var distance = _map.DistanceBetween(node, target);

            var isVisible = distance <= VISIBLE_RANGE;
            return isVisible;
        }

        public bool Test(IActor actor, ILogicState currentState, ILogicStateData data)
        {
            // На каждом шаге осматриваем окрестности
            // на предмет нарушителей.
            var intruders = CheckForIntruders(actor);

            var orderedIntruders = intruders.OrderBy(x => _map.DistanceBetween(actor.Node, x.Node));
            var nearbyIntruder = orderedIntruders.FirstOrDefault();

            if (nearbyIntruder == null)
            {
                return false;
            }

            return true;
        }

        public ILogicStateData CreateData(IActor actor)
        {
            return new EmptyLogicTriggerData();
        }
    }
}
