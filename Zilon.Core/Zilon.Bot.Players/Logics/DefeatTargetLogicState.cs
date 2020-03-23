using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Props;
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
            if (sectorManager is null)
            {
                throw new ArgumentNullException(nameof(sectorManager));
            }

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

            var act = SelectBestAct(actor);

            var isInDistance = act.CheckDistance(actor.Node, target.Node, _map);
            var targetIsOnLine = _map.TargetIsOnLine(actor.Node, target.Node);

            return isInDistance && targetIsOnLine;
        }

        private ITacticalAct SelectBestAct(IActor actor)
        {
            var availableActs = actor.Person.TacticalActCarrier.Acts
                .Where(x => x.CurrentCooldown == null || x.CurrentCooldown == 0)
                .Where(x => TacticalActIsAvailableByConstrains(x, actor.Person));

            return availableActs.FirstOrDefault();
        }

        private bool TacticalActIsAvailableByConstrains(ITacticalAct tacticalAct, IPerson person)
        {
            if (tacticalAct.Constrains is null)
            {
                // Если нет никаких ограничений, то действие доступно в любом случае.
                return true;
            }

            if (tacticalAct.Constrains.PropResourceType is null)
            {
                // Если нет ограничений по ресурсам, то действие доступно.
                return true;
            }

            // Проверяем наличие ресурсов в нужном количестве.
            // Проверка осуществляется в хранилище, указанном параметром.

            if (!person.HasInventory)
            {
                // Персонажи бе инвентаря не могут применять действия,
                // для которых нужны ресурсы.
                return false;
            }

            var propResourceType = tacticalAct.Constrains.PropResourceType;
            var propResourceCount = tacticalAct.Constrains.PropResourceCount.Value;
            if (CheckPropResource(person.Inventory, propResourceType, propResourceCount))
            {
                return true;
            }

            return false;
        }

        private bool CheckPropResource(IPropStore inventory,
            string usedPropResourceType,
            int usedPropResourceCount)
        {
            var props = inventory.CalcActualItems();
            var propResources = new List<Resource>();
            foreach (var prop in props)
            {
                var propResource = prop as Resource;
                if (propResource == null)
                {
                    continue;
                }

                if (propResource.Scheme.Bullet?.Caliber == usedPropResourceType)
                {
                    propResources.Add(propResource);
                }
            }

            var preferredPropResource = propResources.FirstOrDefault();

            return preferredPropResource != null && preferredPropResource.Count >= usedPropResourceCount;
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
                var act = GetUsedActs(actor).First();
                var attackTask = new AttackTask(actor, _target, act, _actService);
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

        private static IEnumerable<ITacticalAct> GetUsedActs(IActor actor)
        {
            if (actor.Person.EquipmentCarrier == null)
            {
                yield return actor.Person.TacticalActCarrier.Acts.First();
            }
            else
            {
                var usedEquipmentActs = false;
                var slots = actor.Person.EquipmentCarrier.Slots;
                for (var i = 0; i < slots.Length; i++)
                {
                    var slotEquipment = actor.Person.EquipmentCarrier[i];
                    if (slotEquipment == null)
                    {
                        continue;
                    }

                    if ((slots[i].Types & EquipmentSlotTypes.Hand) == 0)
                    {
                        continue;
                    }

                    var equipmentActs = from act in actor.Person.TacticalActCarrier.Acts
                                        where act.Equipment == slotEquipment
                                        select act;

                    var usedAct = equipmentActs.FirstOrDefault();

                    if (usedAct != null)
                    {
                        usedEquipmentActs = true;

                        yield return usedAct;
                    }
                }

                if (!usedEquipmentActs)
                {
                    yield return actor.Person.TacticalActCarrier.Acts.First();
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
