using System;
using System.Collections.Generic;
using System.Linq;
using Zilon.Core.Persons;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour.Bots
{
    //TODO Учесть, что в один ход другой актёр может занять целевой узел.
    //TODO Учесть, что при малом расстоянии до цели нужно строить путь каждый ход
    //Иначе не получится догнать нарушителя.
    public class PatrolLogic : IBotLogic
    {
        private const int PERSIUT_COUNTER = 3;
        //TODO Дальность видимости вынести в схему персонажа и, зтем, в пересчитанном состоянии в актёра.
        private const int VISIBILITY_RANGE = 5;
        private readonly IActor _actor;
        private readonly IPatrolRoute _patrolRoute;
        private readonly IMap _map;
        private readonly IActorManager _actorList;
        private readonly IDecisionSource _decisionSource;
        private MoveTask _moveTask;
        private IdleTask _idleTask;
        private PatrolMode _mode;
        private IAttackTarget _targetIntruder;
        private int _persuitCounter;
        private int? _patrolPointIndex;

        public PatrolLogic(IActor actor,
            IPatrolRoute patrolRoute,
            IMap map,
            IActorManager actors,
            IDecisionSource decisionSource)
        {

            _actor = actor;
            _patrolRoute = patrolRoute;
            _map = map;
            _actorList = actors;
            _decisionSource = decisionSource;

            _persuitCounter = PERSIUT_COUNTER;
        }

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
            else if (_idleTask?.IsComplete == true)
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
                _idleTask = new IdleTask(_actor, _decisionSource);
            }
            return _idleTask;
        }

        private IActorTask HandlePersiutMode()
        {
            var isAttackAllowed = CheckAttackAvailability(_targetIntruder);
            if (isAttackAllowed)
            {
                var attackTask = new AttackTask(_actor, _targetIntruder, _decisionSource);
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
            var actorNode = (HexNode)_actor.Node;
            var targetNode = (HexNode)targetIntruder.Node;

            if (_actor.Person.TacticalActCarrier != null)
            {
                var actCarrier = _actor.Person.TacticalActCarrier;
                var act = actCarrier.Acts.First();

                var isInDistance = act.CheckDistance(actorNode.CubeCoords, targetNode.CubeCoords);

                return isInDistance;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private IActorTask HandleBypassMode()
        {
            if (_moveTask == null)
            {
                if (_patrolPointIndex == null)
                {
                    var currentPatrolPointIndex = CalcCurrentPatrolPointIndex();

                    IMapNode nextPatrolPoint;
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

                _moveTask = null;
                _idleTask = new IdleTask(_actor, _decisionSource);
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
            foreach (var target in _actorList.Actors)
            {
                if (target.Owner == _actor.Owner)
                {
                    continue;
                }

                if (target.IsDead)
                {
                    continue;
                }

                var isVisible = CheckTargetVisible(_actor, target);
                if (!isVisible)
                {
                    continue;
                }

                foundIntruders.Add(target);
            }

            return foundIntruders.ToArray();
        }

        private bool CheckTargetVisible(IActor actor, IAttackTarget target)
        {
            var actorNode = (HexNode)actor.Node;
            var targetNode = (HexNode)target.Node;
            var distance = actorNode.CubeCoords.DistanceTo(targetNode.CubeCoords);

            var isVisible = distance <= VISIBILITY_RANGE;
            return isVisible;
        }

        private enum PatrolMode
        {
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
