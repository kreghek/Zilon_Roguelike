﻿using System;
using System.Linq;

using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Triggers
{
    public static class SurvivalHazardTriggerHelper
    {
        public static bool TestHazardAndResource(
            IActor actor,
            ActorTaskContext taskContext,
            ILogicStrategyData strategyData,
            SurvivalStatType statType,
            ConsumeCommonRuleType ruleType)
        {
            var hasHazardEffect = HasCondition(actor, statType);
            if (!hasHazardEffect)
            {
                return false;
            }

            var resource = ResourceToReduceHazard(actor, taskContext, ruleType);
            if (resource is null)
            {
                strategyData.ResourceToReduceHazard = null;
                return false;
            }

            strategyData.ResourceToReduceHazard = resource;

            return true;
        }

        private static bool HasCondition(IActor actor, SurvivalStatType survivalStatType)
        {
            if (actor is null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            var сonditionsModule = actor.Person.GetModuleSafe<IConditionsModule>();
            if (сonditionsModule is null)
            {
                return false;
            }

            var hazardEffect = сonditionsModule.Items.OfType<SurvivalStatHazardCondition>()
                .SingleOrDefault(x => x.Type == survivalStatType && x.Level == SurvivalStatHazardLevel.Strong);
            if (hazardEffect == null)
            {
                return false;
            }

            return true;
        }

        private static Resource ResourceToReduceHazard(IActor actor, ActorTaskContext taskContext,
            ConsumeCommonRuleType ruleType)
        {
            if (actor is null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            var props = actor.Person.GetModule<IInventoryModule>().CalcActualItems();
            var resources = props.OfType<Resource>();

            var bestResource = ResourceFinder.FindBestConsumableResourceByRule(
                actor,
                taskContext,
                resources,
                ruleType);

            return bestResource;
        }
    }
}