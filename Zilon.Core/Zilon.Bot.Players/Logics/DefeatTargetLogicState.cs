using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Persons;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players.Logics
{
    public sealed class DefeatTargetLogicState : LogicStateBase
    {
        private const int REFRESH_COUNTER_VALUE = 3;

        private IAttackTarget _target;

        private int _refreshCounter;

        private MoveTask _moveTask;

        private readonly ISectorMap _map;
        private readonly ITacticalActUsageService _actService;
        private readonly IActorManager _actorManager;

        public DefeatTargetLogicState(ISectorManager sectorManager,
                                      ITacticalActUsageService actService,
                                      IActorManager actorManager)
        {
            _map = sectorManager.CurrentSector.Map;
            _actService = actService ?? throw new ArgumentNullException(nameof(actService));
            _actorManager = actorManager ?? throw new ArgumentNullException(nameof(actorManager));
        }

        private IAttackTarget GetTarget(IActor actor)
        {
            //TODO Убрать дублирование кода с IntruderDetectedTrigger
            // Этот фрагмент уже однажды был использован неправильно,
            // что привело к трудноуловимой ошибке.
            var intruders = CheckForIntruders(actor);

            var orderedIntruders = intruders.OrderBy(x => _map.DistanceBetween(actor.Node, x.Node));
            var nearbyIntruder = orderedIntruders.FirstOrDefault();

            return nearbyIntruder;
        }

        private IEnumerable<IActor> CheckForIntruders(IActor actor)
        {
            foreach (var target in _actorManager.Items)
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

                yield return target;
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

            var isInDistance = act.CheckDistance(actor.Node, target.Node, _map);
            var targetIsOnLine = _map.TargetIsOnLine(actor.Node, target.Node);

            return isInDistance && targetIsOnLine;
        }

        public override IActorTask GetTask(IActor actor, ILogicStrategyData strategyData)
        {
            if (_target == null)
            {
                _target = GetTarget(actor);
            }

            var targetCanBeDamaged = _target.CanBeDamaged();
            if (!targetCanBeDamaged)
            {
                Complete = true;
                return null;
            }

            var isAttackAllowed = CheckAttackAvailability(actor, _target);
            if (isAttackAllowed)
            {
                var attackTask = new AttackTask(actor, _target, _actService);
                return attackTask;
            }
            else
            {
                // Маршрут до цели обновляем каждые 3 хода.
                // Для оптимизации.
                // Эффект потери цели.

                if (_refreshCounter > 0 && _moveTask?.CanExecute() == true)
                {
                    _refreshCounter--;
                    return _moveTask;
                }
                else
                {
                    _refreshCounter = REFRESH_COUNTER_VALUE;
                    var targetIsOnLine = _map.TargetIsOnLine(actor.Node, _target.Node);

                    if (targetIsOnLine)
                    {
                        _moveTask = new MoveTask(actor, _target.Node, _map);
                        return _moveTask;
                    }
                    else
                    {
                        // Цел за пределами видимости. Считается потерянной.
                        return null;
                    }
                }
            }
        }

        protected override void ResetData()
        {
            _refreshCounter = 0;
            _target = null;
            _moveTask = null;
        }
    }
}
