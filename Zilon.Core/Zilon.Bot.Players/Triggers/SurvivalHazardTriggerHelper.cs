﻿using System;
using System.Linq;

using JetBrains.Annotations;

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
            [NotNull] IActor actor,
            ActorTaskContext taskContext,
            [NotNull] ILogicStrategyData strategyData,
            SurvivalStatType statType,
            ConsumeCommonRuleType ruleType)
        {
            var hasHazardEffect = HasEffect(actor, statType);
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

        private static bool HasEffect(IActor actor, SurvivalStatType survivalStatType)
        {
            if (actor is null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            var сonditionModule = actor.Person.GetModuleSafe<IConditionModule>();
            if (сonditionModule is null)
            {
                return false;
            }

            var hazardEffect = сonditionModule.Items.OfType<SurvivalStatHazardEffect>()
                .SingleOrDefault(x => x.Type == survivalStatType && x.Level == SurvivalStatHazardLevel.Strong);
            if (hazardEffect == null)
            {
                return false;
            }

            return true;
        }

        [CanBeNull]
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