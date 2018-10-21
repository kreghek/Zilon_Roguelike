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
        private const int PursuitCounter = 3;
        //TODO Дальность видимости вынести в схему персонажа и, затем, в пересчитанном состоянии в актёра.
        private const int VisibilityRange = 5;
        private readonly IActor _actor;
        private readonly IPatrolRoute _patrolRoute;
        private readonly IMap _map;
        private readonly IActorManager _actorList;
        private readonly IDecisionSource _decisionSource;
        private readonly ITacticalActUsageService _actService;
        private MoveTask _moveTask;
        private IdleTask _idleTask;
        private PatrolMode _mode;
        private IAttackTarget _targetIntruder;
        private int _pursuitCounter;
        private int? _patrolPointIndex;

        public PatrolLogic(IActor actor,
            IPatrolRoute patrolRoute,
            IMap map,
            IActorManager actors,
            IDecisionSource decisionSource,
            ITacticalActUsageService actService)
        {

            _actor = actor;
            _patrolRoute = patrolRoute;
            _map = map;
            _actorList = actors;
            _decisionSource = decisionSource;
            _actService = actService;
            _pursuitCounter = PursuitCounter;
        }

        public IActorTask GetCurrentTask()
        {
            // На каждом шаге осматриваем окрестности
            // на предмет нарушителей.
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
                    return HandlePersuitMode();

                case PatrolMode.Idle:
                    return HandleIdleMode();

                default:
                    throw new InvalidOperationException($"Неизвестный режим патрулирования {_mode}");
            }
        }

        //TODO На этот метод нужен тест.
        private IActorTask HandleIdleMode()
        {
            if (_idleTask == null)
            {
                _idleTask = new IdleTask(_actor, _decisionSource);
            }
            else
            {
                // Ожидание окончено, нужно двигаться к следующей точке.
                if (_idleTask.IsComplete)
                {
                    _idleTask = null;
                    return HandleBypassMode();
                }
            }

            return _idleTask;
        }

        private IActorTask HandlePersuitMode()
        {
            var isAttackAllowed = CheckAttackAvailability(_targetIntruder);
            if (isAttackAllowed)
            {
                var attackTask = new AttackTask(_actor, _targetIntruder, _actService, _map);
                return attackTask;
            }
            else
            {
                // Маршрут до цели обновляем каждые 3 хода.
                // Для оптимизации.
                // Эффект потери цели.

                //TODO Сделать тест аналогичный GetActorTasks_PatrolsTryToAttackEnemy_ReturnsMoveTask
                if (_pursuitCounter > 0 && _moveTask != null)
                {
                    _pursuitCounter--;
                    return _moveTask;
                }
                else
                {
                    RefreshPursuitCounter();
                    _moveTask = new MoveTask(_actor, _targetIntruder.Node, _map);
                    return _moveTask;
                }
            }
        }

        private void RefreshPursuitCounter()
        {
            _pursuitCounter = PursuitCounter;
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
                var targetIsOnLine = MapHelper.CheckNodeAvailability(_map, actorNode, targetNode);

                return isInDistance && targetIsOnLine;
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
                return CreateBypassMoveTask();
            }
            else
            {
                if (!_moveTask.IsComplete)
                {
                    // Если команда на перемещение к целевой точке патруля не закончена, тогда продолжаем её.
                    return _moveTask;
                }

                // Команда на перемещение к целевой точке патруля закончена.
                // Нужно выбрать следующую целевую точку и создать команду на простой.
                _patrolPointIndex++;
                if (_patrolPointIndex >= _patrolRoute.Points.Length)
                {
                    _patrolPointIndex = 0;
                }

                _moveTask = null;
                _idleTask = new IdleTask(_actor, _decisionSource);
                _mode = PatrolMode.Idle;
                return _idleTask;
            }
        }

        /// <summary>
        /// Создаёт задачу на перемещение для обхода патрульных точек.
        /// </summary>
        /// <returns> Возвращает команду на перемещение. </returns>
        private IActorTask CreateBypassMoveTask()
        {
            // Если ещё не известна целевая точка патруля
            if (_patrolPointIndex == null)
            {
                var currentPatrolPointIndex = CalcCurrentPatrolPointIndex();

                IMapNode nextPatrolPoint;
                if (currentPatrolPointIndex != null)
                {
                    // Актёр уже стоит в одной из точек патруля.
                    nextPatrolPoint = GetNextPatrolPointFromPatrolPoint(currentPatrolPointIndex.Value);
                }
                else
                {
                    // Актёр не на контрольной точке.
                    // Возвращаемся на маршрут патруля.

                    nextPatrolPoint = GetNextPatrolPointFromField();
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

        /// <summary>
        /// Рассчёт следующей контрольной точки, если актёр стоит в поле (не на маршруте патруля).
        /// </summary>
        /// <returns> Возвращает узел карты, представляющий следующую контрольную точку патруля. </returns>
        private IMapNode GetNextPatrolPointFromField()
        {
            var actualPatrolPoints = CalcActualRoutePoints();
            var nearbyPatrolPoint = CalcNearbyPatrolPoint(actualPatrolPoints);
            var nextPatrolPoint = nearbyPatrolPoint;
            return nextPatrolPoint;
        }

        /// <summary>
        /// Расчёт следующей контрольной точке патруля из указанной.
        /// </summary>
        /// <param name="currentPatrolPointIndex"> Текущая точка патруля. </param>
        /// <returns> Возвращает узел карты, представляющий следующую контрольную точку патруля. </returns>
        private IMapNode GetNextPatrolPointFromPatrolPoint(int currentPatrolPointIndex)
        {
            _patrolPointIndex = currentPatrolPointIndex + 1;
            if (_patrolPointIndex >= _patrolRoute.Points.Length)
            {
                _patrolPointIndex = 0;
            }

            var nextPatrolPoint = _patrolRoute.Points[_patrolPointIndex.Value];
            return nextPatrolPoint;
        }

        private int? CalcCurrentPatrolPointIndex()
        {
            int? currentIndex = null;
            for (var i = 0; i < _patrolRoute.Points.Length; i++)
            {
                var routeNode = (HexNode)_patrolRoute.Points[i];
                var actorNode = (HexNode)_actor.Node;
                if (!HexNodeHelper.EqualCoordinates(routeNode, actorNode))
                {
                    continue;
                }

                currentIndex = i;
                break;
            }

            return currentIndex;
        }

        /// <summary>
        /// Получение точек патруля, которые можно обходить.
        /// </summary>
        /// <returns> Набор узлов карты. </returns>
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
            foreach (var target in _actorList.Items)
            {
                if (target.Owner == _actor.Owner)
                {
                    continue;
                }

                if (target.Person.Survival.IsDead)
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

            var isVisible = distance <= VisibilityRange;
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