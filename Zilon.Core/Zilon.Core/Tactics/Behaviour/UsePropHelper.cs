using System;
using System.Diagnostics;
using System.Linq;

using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Helper class to accumulate code for prop usage.
    /// </summary>
    public static class UsePropHelper
    {
        /// <summary>
        /// Checks a prop allowed to use by restriction rules.
        /// </summary>
        /// <param name="usedProp">The prop to use.</param>
        /// <param name="actor">The actor using the prop.</param>
        /// <param name="context"> The context of usage. </param>
        /// <returns>Returns true if usage allowed. Otherwise - false.</returns>
        public static bool CheckPropAllowedByRestrictions(IProp usedProp, IActor actor, IActorTaskContext context)
        {
            var restrictions = usedProp.Scheme.Use?.Restrictions;
            if (restrictions is null)
            {
                // Prop without restrictions automaticcaly allowed.
                return true;
            }

            if (restrictions.Items is null)
            {
                Debug.Fail("The restriction items can not be null.");

                // There are restrictions but items is null. This is incorrect.
                // We know it restricted but don't know how to check it.
                // So prop is now allowed at all.
                return false;
            }

            foreach (var restriction in restrictions.Items)
            {
                if (restriction is null)
                {
                    continue;
                }

                var isAllowed = CheckPropAllowedByRestriction(restriction.Type, actor, context);

                if (!isAllowed)
                {
                    return false;
                }
            }

            // No restrictions were fired means usage allowed.
            return true;
        }

        private static bool CheckPropAllowedByRestriction(UsageRestrictionRule restrictionType, IActor actor,
            IActorTaskContext context)
        {
            switch (restrictionType)
            {
                case UsageRestrictionRule.Undefined:
                    throw new InvalidOperationException(
                        $"Restriction type is {nameof(UsageRestrictionRule.Undefined)}.");

                case UsageRestrictionRule.NoStarvation:
                    return !IsRestrictedByStarvation(actor);

                case UsageRestrictionRule.NoDehydration:
                    return !IsRestrictedByDehydration(actor);

                case UsageRestrictionRule.NoOverdose:
                    return !IsRestrictedByOverdose(actor);

                case UsageRestrictionRule.OnlySafeEnvironment:

                    var hostilesinSector = context.Sector.ActorManager.Items
                        .Where(x => x != actor && actor.Person.Fraction.GetRelation(x.Person.Fraction) ==
                            FractionRelation.Enmity);

                    return !hostilesinSector.Any();

                default:
                    throw new NotSupportedException($"Restriction {restrictionType} is unknown.");
            }
        }

        private static bool CheckConditionWithMaxLevel(IActor actor, SurvivalStatType effectType)
        {
            var isRestricted = false;

            var сonditionModule = actor.Person.GetModuleSafe<IConditionModule>();
            if (сonditionModule != null)
            {
                var searchingСondition = сonditionModule.Items.OfType<SurvivalStatHazardEffect>()
                    .SingleOrDefault(x => x.Type == effectType && x.Level == SurvivalStatHazardLevel.Max);

                if (searchingСondition != null)
                {
                    isRestricted = true;
                }
            }

            return isRestricted;
        }

        private static bool IsRestrictedByDehydration(IActor actor)
        {
            return CheckConditionWithMaxLevel(actor, SurvivalStatType.Hydration);
        }

        private static bool IsRestrictedByOverdose(IActor actor)
        {
            return CheckConditionWithMaxLevel(actor, SurvivalStatType.Intoxication);
        }

        private static bool IsRestrictedByStarvation(IActor actor)
        {
            return CheckConditionWithMaxLevel(actor, SurvivalStatType.Satiety);
        }
    }
}