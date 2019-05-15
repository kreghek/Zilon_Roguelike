using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Bot.Players.Triggers
{
    public class LowHpAndHasResourceTrigger : ILogicStateTrigger
    {
        public ILogicStateData CreateData(IActor actor)
        {
            return new EmptyLogicTriggerData();
        }

        public bool Test(IActor actor, ILogicState currentState, ILogicStateData data)
        {
            var hpStat = actor.Person.Survival.Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Health);
            var hpStatCoeff = (float)hpStat.Value / (hpStat.Range.Max - hpStat.Range.Min);
            var isLowHp = hpStatCoeff <= 0.5f;
            if (!isLowHp)
            {
                return false;
            }

            //

            var props = actor.Person.Inventory.CalcActualItems();
            var resources = props.OfType<Resource>();
            var foundHealResources = FindHealResources(resources);

            var orderedHealResources = foundHealResources.OrderByDescending(x => x.Rule.Level);
            var bestHealResource = foundHealResources.FirstOrDefault();

            if (bestHealResource == null)
            {
                return false;
            }

            return true;
        }

        private static IEnumerable<HealSelection> FindHealResources(IEnumerable<Resource> resources)
        {
            foreach (var resource in resources)
            {
                var rule = resource.Scheme.Use.CommonRules
                    .SingleOrDefault(x => x.Type == ConsumeCommonRuleType.Health && x.Direction == PersonRuleDirection.Positive);

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
    }
}
