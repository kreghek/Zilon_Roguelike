using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Persons;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players.Triggers
{
    public class IntruderDetectedTrigger : ILogicStateTrigger
    {
        private readonly ISectorManager _sectorManager;
        private readonly ISectorMap _map;

        public IntruderDetectedTrigger(ISectorManager sectorManager)
        {
            _sectorManager = sectorManager ?? throw new System.ArgumentNullException(nameof(sectorManager));
            _map = sectorManager.CurrentSector.Map;
        }

        private IActor[] CheckForIntruders(IActor actor)
        {
            var foundIntruders = new List<IActor>();
            foreach (var target in _sectorManager.CurrentSector.ActorManager.Items)
            {
                if (target.Owner == actor.Owner)
                {
                    continue;
                }

                if (target.Person.CheckIsDead())
                {
                    continue;
                }

                var isVisible = LogicHelper.CheckTargetVisible(_map, actor.Node, target.Node);
                if (!isVisible)
                {
                    continue;
                }

                foundIntruders.Add(target);
            }

            return foundIntruders.ToArray();
        }

        public bool Test(IActor actor, ILogicState currentState, ILogicStrategyData strategyData)
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