﻿using System;
using System.Linq;

using Zilon.Bot.Players.Triggers;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Logics
{
    public class HealSelfLogicState : LogicStateBase
    {
        public override IActorTask GetTask(IActor actor, ISectorTaskSourceContext context,
            ILogicStrategyData strategyData)
        {
            if (actor is null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var hpStat = actor.Person.GetModule<ISurvivalModule>().Stats
                .SingleOrDefault(x => x.Type == SurvivalStatType.Health);
            if (hpStat == null)
            {
                Complete = true;
                return null;
            }

            var hpStatCoeff = (float)hpStat.Value / (hpStat.Range.Max - hpStat.Range.Min);
            var isLowHp = hpStatCoeff <= 0.5f;
            if (!isLowHp)
            {
                Complete = true;
                return null;
            }

            var props = actor.Person.GetModule<IInventoryModule>().CalcActualItems();
            var resources = props.OfType<Resource>();
            var taskContext = new ActorTaskContext(context.Sector);
            var bestResource = ResourceFinder.FindBestConsumableResourceByRule(actor, taskContext, resources,
                ConsumeCommonRuleType.Health);

            if (bestResource == null)
            {
                Complete = true;
                return null;
            }

            return new UsePropTask(actor, taskContext, bestResource);
        }

        protected override void ResetData()
        {
            // Внутреннего состояния нет.
        }
    }
}