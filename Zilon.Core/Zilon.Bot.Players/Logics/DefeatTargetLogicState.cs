﻿using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players.Logics
{
    public sealed class DefeatTargetLogicState : LogicStateBase
    {
        private const int REFRESH_COUNTER_VALUE = 3;

        private readonly ITacticalActUsageService _actService;

        private MoveTask _moveTask;

        private int _refreshCounter;

        public DefeatTargetLogicState(ITacticalActUsageService actService)
        {
            _actService = actService ?? throw new ArgumentNullException(nameof(actService));
        }

        public override IActorTask GetTask(IActor actor, ISectorTaskSourceContext context,
            ILogicStrategyData strategyData)
        {
            var triggerTarget = strategyData.TriggerIntuder;

            if (triggerTarget == null)
            {
                throw new InvalidOperationException(
                    $"Assign {nameof(strategyData.TriggerIntuder)} with not null valie in related trigger.");
            }

            var targetCanBeDamaged = triggerTarget.CanBeDamaged();
            if (!targetCanBeDamaged)
            {
                Complete = true;
                return null;
            }

            var attackParams = CheckAttackAvailability(actor, triggerTarget, context.Sector.Map);
            if (attackParams.IsAvailable)
            {
                var act = attackParams.CombatAct;

                var taskContext = new ActorTaskContext(context.Sector);

                var attackTask = new AttackTask(actor, taskContext, triggerTarget, act, _actService);
                return attackTask;
            }

            // Маршрут до цели обновляем каждые 3 хода.
            // Для оптимизации.
            // Эффект потери цели.
            if (_refreshCounter > 0 && _moveTask?.CanExecute() == true)
            {
                _refreshCounter--;
                return _moveTask;
            }

            var map = context.Sector.Map;
            _refreshCounter = REFRESH_COUNTER_VALUE;
            var targetIsOnLine = map.TargetIsOnLine(actor.Node, triggerTarget.Node);

            if (targetIsOnLine)
            {
                var taskContext = new ActorTaskContext(context.Sector);
                var moveModule = actor.Person.GetModule<IMovingModule>();
                var moveCost = moveModule.CalculateCost();

                _moveTask = new MoveTask(actor, taskContext, triggerTarget.Node, map, moveCost);
                return _moveTask;
            }

            // Цел за пределами видимости. Считается потерянной.
            return null;
        }

        protected override void ResetData()
        {
            _refreshCounter = 0;
            _moveTask = null;
        }

        private static AttackParams CheckAttackAvailability(IActor actor, IAttackTarget target, ISectorMap map)
        {
            var combatActModule = actor.Person.GetModuleSafe<ICombatActModule>();
            if (combatActModule is null)
            {
                throw new NotSupportedException();
            }

            var inventory = actor.Person.GetModuleSafe<IInventoryModule>();

            var acts = combatActModule.GetCurrentCombatActs();
            var act = SelectActHelper.SelectBestAct(acts, inventory);

            var isInDistance = act.CheckDistance(actor.Node, target.Node, map);
            var targetIsOnLine = map.TargetIsOnLine(actor.Node, target.Node);

            var attackParams = new AttackParams
            {
                IsAvailable = isInDistance && targetIsOnLine,
                CombatAct = act
            };

            return attackParams;
        }

        private record AttackParams
        {
            public ICombatAct CombatAct { get; init; }
            public bool IsAvailable { get; init; }
        }
    }
}