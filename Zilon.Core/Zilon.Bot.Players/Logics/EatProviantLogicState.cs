using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Logics
{
    public class EatProviantLogicState : ILogicState
    {
        public bool Complete { get; private set; }

        public ILogicStateData CreateData(IActor actor)
        {
            return new EmptyLogicTriggerData();
        }

        public IActorTask GetTask(IActor actor, ILogicStateData data)
        {
            var eatFoodTask = CheckHazard(actor, SurvivalStatType.Satiety, ConsumeCommonRuleType.Satiety);
            if (eatFoodTask != null)
            {
                return eatFoodTask;
            }

            var drinkWaterTask = CheckHazard(actor, SurvivalStatType.Water, ConsumeCommonRuleType.Thirst);
            if (drinkWaterTask != null)
            {
                return drinkWaterTask;
            }

            Complete = true;
            return null;
        }

        private static IEnumerable<HealSelection> FindFoodResources(IEnumerable<Resource> resources, ConsumeCommonRuleType foodType)
        {
            foreach (var resource in resources)
            {
                var rule = resource.Scheme.Use.CommonRules
                    .SingleOrDefault(x => x.Type == foodType && x.Direction == PersonRuleDirection.Positive);

                if (rule != null)
                {
                    yield return new HealSelection
                    {
                        Resource = resource,
                        Rule = rule
                    };
                }
            }
        }

        private class HealSelection
        {
            public Resource Resource;
            public ConsumeCommonRule Rule;
        }

        private UsePropTask CheckHazard(IActor actor, SurvivalStatType hazardType, ConsumeCommonRuleType resourceType)
        {
            var hazardEffect = actor.Person.Effects.Items.OfType<SurvivalStatHazardEffect>()
                .SingleOrDefault(x => x.Type == hazardType);
            if (hazardEffect == null)
            {
                return null;
            }

            var props = actor.Person.Inventory.CalcActualItems();
            var resources = props.OfType<Resource>();
            var foundHealResources = FindFoodResources(resources, resourceType);

            var orderedHealResources = foundHealResources.OrderByDescending(x => x.Rule.Level);
            var bestHealResource = foundHealResources.FirstOrDefault();

            if (bestHealResource == null)
            {
                return null;
            }

            return new UsePropTask(actor, bestHealResource.Resource);
        }
    }
}
