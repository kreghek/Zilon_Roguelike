using System;
using System.Linq;

using Zilon.Core.Persons;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players.Logics
{
    public sealed class DefeatTargetLogicState : ILogicState
    {
        private readonly ISectorMap _map;
        private readonly ITacticalActUsageService _actService;

        public DefeatTargetLogicState(ISectorMap map,
                                      ITacticalActUsageService actService)
        {
            _map = map ?? throw new ArgumentNullException(nameof(map));
            _actService = actService ?? throw new ArgumentNullException(nameof(actService));
        }

        public bool Complete { get; }

        public ILogicStateData CreateData(IActor actor)
        {
            var target = GetTarget();
            return new DefeateTargetLogicData(target);
        }

        private IAttackTarget GetTarget()
        {
            throw new NotImplementedException();
        }


        public IActorTask GetTask(IActor actor, ILogicStateData data)
        {
            var logicData = (DefeateTargetLogicData)data;

            var isAttackAllowed = CheckAttackAvailability(actor, logicData.Target);
            if (isAttackAllowed)
            {
                var attackTask = new AttackTask(actor, logicData.Target, _actService);
                return attackTask;
            }
            else
            {
                // Маршрут до цели обновляем каждые 3 хода.
                // Для оптимизации.
                // Эффект потери цели.

                if (logicData.RefreshCounter > 0 && logicData.MoveTask?.CanExecute() == true)
                {
                    logicData.RefreshCounter--;
                    return logicData.MoveTask;
                }
                else
                {
                    logicData.MoveTask = new MoveTask(actor, logicData.Target.Node, _map);
                    return logicData.MoveTask;
                }
            }
        }

        private bool CheckAttackAvailability(IActor actor, IAttackTarget target)
        {
            if (actor.Person.TacticalActCarrier == null)
            {
                throw new NotSupportedException();
            }

            var actCarrier = actor.Person.TacticalActCarrier;
            var act = actCarrier.Acts.First();

            var isInDistance =  act.CheckDistance(actor.Node, target.Node, _map);
            var targetIsOnLine = _map.TargetIsOnLine(actor.Node, target.Node);

            return isInDistance && targetIsOnLine;
        }

        public bool CanGetTask(IActor actor, ILogicStateData data)
        {
            var logicData = (DefeateTargetLogicData)data;

            var targetCanBeDamaged = logicData.Target.CanBeDamaged();
            var canAct = CheckAttackAvailability(actor, logicData.Target);

            return targetCanBeDamaged && canAct;
        }
    }
}
