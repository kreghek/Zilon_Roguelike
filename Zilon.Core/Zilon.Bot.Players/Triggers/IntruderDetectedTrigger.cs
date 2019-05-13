using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

using Zilon.Bot.Players.Logics;

namespace Zilon.Bot.Players.Triggers
{
    public class IntruderDetectedTrigger: ILogicStateTrigger
    {
        private const int VISIBLE_RANGE = 5;
        private readonly IActor _actor;
        private readonly IActorManager _actorManager;
        private readonly ISectorMap _map;
        private readonly IPlayer _player;

        public IntruderDetectedTrigger(IActor actor, IActorManager actorManager, ISectorMap map, IPlayer player)
        {
            _actor = actor;
            _actorManager = actorManager;
            _map = map;
            _player = player;
        }

        public ILogicStateData Test()
        {
            // На каждом шаге осматриваем окрестности
            // на предмет нарушителей.
            var intruders = CheckForIntruders();

            var orderedIntruders = intruders.OrderBy(x => _map.DistanceBetween(_actor.Node, x.Node));
            var nearbyIntruder = orderedIntruders.FirstOrDefault();

            if (nearbyIntruder == null)
            {
                return null;
            }

            return new PersuitNearbyIntruderData(nearbyIntruder);
        }

        public ILogicState GenerateLogic(ILogicStateData result)
        {
            var logicResult = result as PersuitNearbyIntruderData;
            if (logicResult == null)
            {
                throw new System.ArgumentException("Результат не соответствует ожидаемому типу.");
            }

            return new DefeatTargetLogicState();
        }

        private IActor[] CheckForIntruders()
        {
            var foundIntruders = new List<IActor>();
            foreach (var target in _actorManager.Items)
            {
                if (target.Owner == _player)
                {
                    continue;
                }

                if (target.Person.Survival.IsDead)
                {
                    continue;
                }

                var isVisible = CheckTargetVisible(_actor.Node, target.Node);
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
    }
}
