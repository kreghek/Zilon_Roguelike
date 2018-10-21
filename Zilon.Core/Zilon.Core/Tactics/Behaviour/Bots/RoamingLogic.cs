using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Persons;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour.Bots
{
    /// <summary>
    /// Логика произвольного брожения.
    /// </summary>
    //TODO Объединить с логикой патрулирования.
    public class RoamingLogic : IBotLogic
    {
        private const int PursuitCounter = 3;
        //TODO Дальность видимости вынести в схему персонажа и, зтем, в пересчитанном состоянии в актёра.
        private const int VisibilityRange = 5;

        private readonly IActor _actor;
        private readonly IMap _map;
        private readonly IActorManager _actorList;
        private readonly IDecisionSource _decisionSource;
        private readonly ITacticalActUsageService _actService;

        private IdleTask _roamingTask;
        private IdleTask _idleTask;
        private MoveTask _moveTask;
        private IdleMode _mode;
        private IAttackTarget _targetIntruder;
        private int _persuitCounter;

        public RoamingLogic(IActor actor,
            IMap map,
            IActorManager actors,
            IDecisionSource decisionSource,
            ITacticalActUsageService actService)
        {

            _actor = actor;
            _map = map;
            _actorList = actors;
            _decisionSource = decisionSource;
            _actService = actService;
            _persuitCounter = PursuitCounter;
        }

        public IActorTask GetCurrentTask()
        {
            // На каждом шаге осматриваем окрестности
            // напредмет нарушителей.
            var intruders = CheckForIntruders();

            var nearbyIntruder = intruders.FirstOrDefault();

            if (nearbyIntruder != null)
            {
                _mode = IdleMode.Pursuit;
                _targetIntruder = nearbyIntruder;
                _idleTask = null;
            }
            else if (_idleTask?.IsComplete == true)
            {
                _mode = IdleMode.Bypass;
                _targetIntruder = null;
                _idleTask = null;
            }

            switch (_mode)
            {
                case IdleMode.Bypass:
                    return HandleBypassMode();

                case IdleMode.Pursuit:
                    return HandlePursuitMode();

                case IdleMode.Idle:
                    return HandleIdleMode();

                default:
                    throw new InvalidOperationException($"Неизвестный режим патруллирования {_mode}");
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

        private IActorTask HandlePursuitMode()
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
                if (_persuitCounter > 0 && _moveTask != null)
                {
                    _persuitCounter--;
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
            _persuitCounter = PursuitCounter;
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
            if (_roamingTask == null)
            {
                return CreateBypassMoveTask();
            }
            else
            {
                if (!_roamingTask.IsComplete)
                {
                    // Если команда на перемещение к целевой точке патруля не закончена, тогда продолжаем её.
                    return _roamingTask;
                }

                _roamingTask = null;
                _idleTask = new IdleTask(_actor, _decisionSource);
                _mode = IdleMode.Idle;
                return _idleTask;
            }
        }

        /// <summary>
        /// Создаёт задачу на произвольное брожение.
        /// </summary>
        /// <returns> Возвращает команду на перемещение. </returns>
        private IActorTask CreateBypassMoveTask()
        {
            _roamingTask = new IdleTask(_actor, _decisionSource);

            return _roamingTask;
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


        private enum IdleMode
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
