using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour.Bots
{
    public class PatrolLogic : IBotLogic
    {
        private const int PERSIUT_COUNTER = 3;
        private readonly IActor _actor;
        private readonly IPatrolRoute _patrolRoute;
        private readonly IMap _map;
        private readonly IActorList _actorList;
        private readonly IDecisionSource _decisionSource;
        private MoveTask _moveTask;
        private PatrolMode _mode;
        private IMapNode _targetPatrolPoint;
        private IAttackTarget _targetIntruder;
        private int _persuitCounter;

        public PatrolLogic(IActor actor, 
            IPatrolRoute patrolRoute,
            IMap map, 
            IActorList actors,
            IDecisionSource decisionSource) {

            _actor = actor;
            _patrolRoute = patrolRoute;
            _map = map;
            _actorList = actors;
            _decisionSource = decisionSource;

            _persuitCounter = PERSIUT_COUNTER;
        }

        public IdleTask _idleTask { get; private set; }

        public IActorTask GetCurrentTask()
        {
            // На каждом шаге осматриваем окрестности
            // напредмет нарушителей.
            var intruders = CheckForIntruders();

            var nearbyIntruder = intruders.FirstOrDefault();

            if (nearbyIntruder != null)
            {
                _mode = PatrolMode.Pursuit;
                _targetIntruder = nearbyIntruder;
                _idleTask = null;
            }

            switch (_mode)
            {
                case PatrolMode.Bypass:
                    return HandleBypassMode();

                case PatrolMode.Pursuit:
                    return HandlePersiutMode();

                case PatrolMode.Idle:
                    return HandleIdleMode();

                default:
                    throw new InvalidOperationException($"Неизвестный режим патруллирования {_mode}");
            }
        }

        private IActorTask HandleIdleMode()
        {
            if (_idleTask != null)
            {
                _idleTask = new IdleTask(_decisionSource);
            }
            return _idleTask;
        }

        private IActorTask HandlePersiutMode()
        {
            var isAttackAllowed = CheckAttackAvailability(_targetIntruder);
            if (isAttackAllowed)
            {
                var attackTask = new AttackTask(_actor, _targetIntruder);
                return attackTask;
            }
            else
            {
                // Маршрут до цели обновляем каждые 3 хода.
                // Для оптимизации.
                // Эффект потери цели.

                if (_persuitCounter > 0)
                {
                    _persuitCounter--;
                    return _moveTask;
                }
                else
                {
                    RefreshPersuiteCounter();
                    _moveTask = new MoveTask(_actor, _targetIntruder.Node, _map);
                    return _moveTask;
                }
            }
        }

        private void RefreshPersuiteCounter()
        {
            _persuitCounter = PERSIUT_COUNTER;
        }

        private bool CheckAttackAvailability(IAttackTarget targetIntruder)
        {
            throw new NotImplementedException();
        }

        private IActorTask HandleBypassMode()
        {
            if (_moveTask == null)
            {
                var nearbyPatrolPoint = CalcNearbyPatrolPoint(_targetPatrolPoint);
                _targetPatrolPoint = nearbyPatrolPoint;

                _moveTask = new MoveTask(_actor, nearbyPatrolPoint, _map);
                return _moveTask;
            }
            else
            {
                if (!_moveTask.IsComplete)
                {
                    return _moveTask;
                }

                _idleTask = new IdleTask(_decisionSource);
                _mode = PatrolMode.Idle;
                return _idleTask;
            }
        }

        private IMapNode CalcNearbyPatrolPoint(IMapNode targetPatrolPoint)
        {
            throw new NotImplementedException();
        }

        private IActor[] CheckForIntruders()
        {
            var foundIntruders = new List<IActor>();
            foreach (var actor in _actorList.Actors)
            {
                if (actor.Person.Player == _actor.Person.Player)
                {
                    continue;
                }

                if (actor.IsDead)
                {
                    continue;
                }

                //TODO Добавить видимость (дальность просмотра)
                foundIntruders.Add(actor);
            }

            return foundIntruders.ToArray();
        }

        private enum PatrolMode {
            /// <summary>
            /// Обход ключевых точек.
            /// </summary>
            Bypass,

            /// <summary>
            /// Преследование нарушителя.
            /// </summary>
            Pursuit,

            /// <summary>
            /// Задержка перед следующим решением.
            /// </summary>
            Idle
        }
    }
}
