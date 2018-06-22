using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour.Bots
{
    //TODO Учесть, что в один ход другой актёр может занять целевой узел.
    //TODO Учесть, что при малом расстоянии до цели нужно строить путь каждый ход
    //Иначе не получится догнать нарушителя.
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
        private IAttackTarget _targetIntruder;
        private int _persuitCounter;
        private int? _patrolPointIndex;

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
                _patrolPointIndex = null;
            }
            else if (_idleTask != null && _idleTask.IsComplete)
            {
                _mode = PatrolMode.Bypass;
                _targetIntruder = null;
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
                if (_patrolPointIndex == null)
                {
                    var currentPatrolPointIndex = CalcCurrentPatrolPointIndex();

                    IMapNode nextPatrolPoint = null;
                    if (currentPatrolPointIndex != null)
                    {
                        _patrolPointIndex = currentPatrolPointIndex + 1;
                        nextPatrolPoint = _patrolRoute.Points[_patrolPointIndex.Value];
                    }
                    else
                    {
                        var actualPatrolPoints = CalcActualRoutePoints();
                        var nearbyPatrolPoint = CalcNearbyPatrolPoint(actualPatrolPoints);
                        nextPatrolPoint = nearbyPatrolPoint;
                    }

                    

                    _moveTask = new MoveTask(_actor, nextPatrolPoint, _map);
                }
                else
                {
                    var targetPatrolPoint = _patrolRoute.Points[_patrolPointIndex.Value];

                    _moveTask = new MoveTask(_actor, targetPatrolPoint, _map);
                }
                
                return _moveTask;
            }
            else
            {
                if (!_moveTask.IsComplete)
                {
                    return _moveTask;
                }
                else
                {
                    _patrolPointIndex++;
                    if (_patrolPointIndex >= _patrolRoute.Points.Count())
                    {
                        _patrolPointIndex = 0;
                    }
                }

                _idleTask = new IdleTask(_decisionSource);
                _mode = PatrolMode.Idle;
                return _idleTask;
            }
        }

        private int? CalcCurrentPatrolPointIndex()
        {
            int? currentIndex = null;
            for (var i = 0; i < _patrolRoute.Points.Count(); i++)
            {
                var routeNode = (HexNode)_patrolRoute.Points[i];
                var actorNode = (HexNode)_actor.Node;
                if (HexNodeHelper.EqualCoordinates(routeNode, actorNode))
                {
                    currentIndex = i;
                    break;
                }
            }

            return currentIndex;
        }

        private HexNode[] CalcActualRoutePoints()
        {
            var hexNodes = _patrolRoute.Points.Cast<HexNode>();
            var actorHexNode = (HexNode)_actor.Node;
            var actualRoutePoints = from node in hexNodes
                                    where !HexNodeHelper.EqualCoordinates(node, actorHexNode)
                                    select node;

            return actualRoutePoints.ToArray();
        }

        private IMapNode CalcNearbyPatrolPoint(IEnumerable<HexNode> routePoints)
        {
            var targets = routePoints;
            var node = (HexNode)_actor.Node;
            var nearbyNode = HexNodeHelper.GetNearbyCoordinates(node, targets);

            if (nearbyNode == null)
            {
                throw new InvalidOperationException("Ближайший узел не найден.");
            }

            return nearbyNode;
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
