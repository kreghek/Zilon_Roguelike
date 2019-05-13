using System;
using System.Linq;
using Zilon.Core.Persons;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players.Logics
{
    public sealed class DefeatTargetLogicState : ILogicState
    {
        private readonly IActor _actor;
        private readonly IAttackTarget _targetIntruder;
        private readonly IDecisionSource _decisionSource;
        private readonly ISectorMap _map;
        private readonly ITacticalActUsageService _actService;
        private MoveTask _moveTask;
        private IdleTask _idleTask;
        private int _pursuitCounter;

        public DefeatTargetLogicState(IActor actor,
                                               IAttackTarget targetIntruder,
                                               IDecisionSource decisionSource,
                                               ISectorMap map,
                                               ITacticalActUsageService actService)
        {
            _actor = actor;
            _targetIntruder = targetIntruder ?? throw new ArgumentNullException(nameof(targetIntruder));
            _decisionSource = decisionSource;
            _map = map;
            _actService = actService;
        }

        public IActorTask GetCurrentTask()
        {
            var isAttackAllowed = CheckAttackAvailability(_targetIntruder);
            if (isAttackAllowed)
            {
                var attackTask = new AttackTask(_actor, _targetIntruder, _actService);
                return attackTask;
            }
            else
            {
                // Маршрут до цели обновляем каждые 3 хода.
                // Для оптимизации.
                // Эффект потери цели.

                if (_pursuitCounter > 0 && _moveTask != null && _moveTask.CanExecute())
                {
                    _pursuitCounter--;
                    return _moveTask;
                }
                else
                {
                    _moveTask = new MoveTask(_actor, _targetIntruder.Node, _map);
                    return _moveTask;
                }
            }
        }

        private bool CheckAttackAvailability(IAttackTarget targetIntruder)
        {
            if (_actor.Person.TacticalActCarrier == null)
            {
                throw new NotSupportedException();
            }

            var actorNode = (HexNode)_actor.Node;
            var targetNode = (HexNode)targetIntruder.Node;

            var actCarrier = _actor.Person.TacticalActCarrier;
            var act = actCarrier.Acts.First();

            var isInDistance =  act.CheckDistance(actorNode.CubeCoords, targetNode.CubeCoords);
            var targetIsOnLine = _map.TargetIsOnLine(actorNode, targetNode);

            return isInDistance && targetIsOnLine;
        }
    }
}
