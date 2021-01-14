using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Задача на использование предмета инвентаря.
    /// </summary>
    public sealed class UsePropTask : OneTurnActorTaskBase
    {
        [ExcludeFromCodeCoverage]
        public UsePropTask(IActor actor,
            IActorTaskContext context,
            IProp usedProp) : base(actor, context)
        {
            UsedProp = usedProp;
        }

        public IProp UsedProp { get; }

        protected override void ExecuteTask()
        {
            var isAllow = CheckPropAllowedByRestrictions(UsedProp, Actor, Context);

            if (!isAllow)
            {
                throw new InvalidOperationException(
                    $"Attempt to use the prop {UsedProp} which restricted in current context.");
            }

            Actor.UseProp(UsedProp);
        }

        private static bool CheckEffectWithMaxLevel(IActor actor, SurvivalStatType effectType)
        {
            var isRestricted = false;

            var effectModule = actor.Person.GetModuleSafe<IEffectsModule>();
            if (effectModule != null)
            {
                var searchingEffect = effectModule.Items.OfType<SurvivalStatHazardEffect>()
                    .SingleOrDefault(x => x.Type == effectType && x.Level == SurvivalStatHazardLevel.Max);

                if (searchingEffect != null)
                {
                    isRestricted = true;
                }
            }

            return isRestricted;
        }

        private static bool CheckPropAllowedByRestrictions(IProp usedProp, IActor actor, IActorTaskContext context)
        {
            var restrictions = usedProp.Scheme.Use.Restrictions;
            if (restrictions != null)
            {
                // Prop without restrictions automaticcaly allowed.
                return true;
            }

            foreach (var restriction in restrictions.Items)
            {
                var isAllowed = CheckPropAllowedByRestriction(restriction.Type, actor, context);

                if (!isAllowed)
                {
                    return false;
                }
            }

            // No restrictions were fired means usage allowed.
            return true;
        }

        private static bool CheckPropAllowedByRestriction(UsageRestrictionType restrictionType, IActor actor, IActorTaskContext context)
        {
            switch (restrictionType)
            {
                case UsageRestrictionType.Undefined:
                    throw new InvalidOperationException(
                        $"Restriction type is {nameof(UsageRestrictionType.Undefined)}.");

                case UsageRestrictionType.NoStarvation:

                    if (IsRestrictedByStarvation(actor))
                    {
                        return false;
                    }

                    break;

                case UsageRestrictionType.NoDehydration:

                    if (IsRestrictedByDehydration(actor))
                    {
                        return false;
                    }

                    break;

                case UsageRestrictionType.NoOverdose:

                    if (IsRestrictedByOverdose(actor))
                    {
                        return false;
                    }

                    break;

                case UsageRestrictionType.OnlySafeEnvironment:

                    var hostilesinSector = context.Sector.ActorManager.Items
                        .Where(x => x != actor && actor.Person.Fraction.GetRelation(x.Person.Fraction) !=
                            FractionRelation.Enmity);
                    if (hostilesinSector.Any())
                    {
                        return false;
                    }

                    break;

                default:
                    throw new NotSupportedException($"Restriction {restrictionType} is unknown.");
            }

            return true;
        }

        private static bool IsRestrictedByDehydration(IActor actor)
        {
            return CheckEffectWithMaxLevel(actor, SurvivalStatType.Hydration);
        }

        private static bool IsRestrictedByOverdose(IActor actor)
        {
            return CheckEffectWithMaxLevel(actor, SurvivalStatType.Intoxication);
        }

        private static bool IsRestrictedByStarvation(IActor actor)
        {
            return CheckEffectWithMaxLevel(actor, SurvivalStatType.Satiety);
        }
    }
}