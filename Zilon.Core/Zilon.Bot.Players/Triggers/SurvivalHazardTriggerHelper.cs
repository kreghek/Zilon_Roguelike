using System;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Bot.Players.Triggers
{
    public static class SurvivalHazardTriggerHelper
    {
        private static bool HasEffect(IActor actor, SurvivalStatType survivalStatType)
        {
            if (actor is null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            var effectsModule = actor.Person.GetModuleSafe<IEffectsModule>();
            if (effectsModule is null)
            {
                return false;
            }

            var hazardEffect = effectsModule.Items.OfType<SurvivalStatHazardEffect>()
                .SingleOrDefault(x => x.Type == survivalStatType && x.Level == SurvivalStatHazardLevel.Strong);
            if (hazardEffect == null)
            {
                return false;
            }

            return true;
        }

        [CanBeNull]
        private static Resource ResourceToReduceHazard(IActor actor, ConsumeCommonRuleType ruleType)
        {
            if (actor is null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            var props = actor.Person.GetModule<IInventoryModule>().CalcActualItems();
            var resources = props.OfType<Resource>();
            var bestResource = ResourceFinder.FindBestConsumableResourceByRule(resources,
                ruleType);

            return bestResource;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods",
            Justification = "All null checks in the top-level Test method.")]
        public static bool TestHazardAndResource(
            [NotNull] IActor actor,
            [NotNull] ILogicStrategyData strategyData,
            SurvivalStatType statType,
            ConsumeCommonRuleType ruleType)
        {
            var hasHazardEffect = HasEffect(actor, statType);
            if (!hasHazardEffect)
            {
                return false;
            }

            var resource = ResourceToReduceHazard(actor, ruleType);
            if (resource is null)
            {
                strategyData.ResourceToReduceHazard = null;
                return false;
            }

            strategyData.ResourceToReduceHazard = resource;

            return true;
        }
    }
}
