using System;
using System.Collections.Generic;
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
        private readonly IActorManager _actorManager;

        public DefeatTargetLogicState(ISectorManager sectorManager,
                                      ITacticalActUsageService actService,
                                      IActorManager actorManager)
        {
            _map = sectorManager.CurrentSector.Map;
            _actService = actService ?? throw new ArgumentNullException(nameof(actService));
            _actorManager = actorManager ?? throw new ArgumentNullException(nameof(actorManager));
        }

        public bool Complete { get; private set; }

        public ILogicStateData CreateData(IActor actor)
        {
            var target = GetTarget(actor);
            return new DefeatTargetLogicData(target);
        }

        private IAttackTarget GetTarget(IActor actor)
        {
            //TODO Убрать дублирование кода с InruderDetectedTrigger
            var intruders = CheckForIntruders(actor);

            var orderedIntruders = intruders.OrderBy(x => _map.DistanceBetween(actor.Node, x.Node));
            var nearbyIntruder = orderedIntruders.FirstOrDefault();

            return nearbyIntruder;
        }

        private IActor[] CheckForIntruders(IActor actor)
        {
            var foundIntruders = new List<IActor>();
            foreach (var target in _actorManager.Items)
            {
                if (target.Owner == actor.Owner)
                {
                    continue;
                }

                if (target.Person.Survival.IsDead)
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


        public IActorTask GetTask(IActor actor, ILogicStateData data)
        {
            var logicData = (DefeatTargetLogicData)data;

            var targetCanBeDamaged = logicData.Target.CanBeDamaged();
            if (!targetCanBeDamaged)
            {
                Complete = true;
                return null;
            }

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
                    var targetIsOnLine = _map.TargetIsOnLine(actor.Node, logicData.Target.Node);

                    if (targetIsOnLine)
                    {
                        logicData.MoveTask = new MoveTask(actor, logicData.Target.Node, _map);
                        return logicData.MoveTask;
                    }
                    else
                    {
                        // Цел за пределами видимости. Считается потерянной.
                        return null;
                    }
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
            var logicData = (DefeatTargetLogicData)data;

            var targetCanBeDamaged = logicData.Target.CanBeDamaged();
            var canAct = CheckAttackAvailability(actor, logicData.Target);

            return targetCanBeDamaged && canAct;
        }
    }
}
