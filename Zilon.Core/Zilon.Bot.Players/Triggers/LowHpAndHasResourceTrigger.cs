using System.Linq;

using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Bot.Players.Triggers
{
    public class LowHpAndHasResourceTrigger : ILogicStateTrigger
    {
        public void Reset()
        {
            // Нет состояния
        }

        public bool Test(IActor actor, ILogicState currentState, ILogicStrategyData strategyData)
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
            var bestResource = ResourceFinder.FindBestConsumableResourceByRule(resources,
                ConsumeCommonRuleType.Health);

            if (bestResource == null)
            {
                return false;
            }

            return true;
        }

        public void Update()
        {
            // Нет состояния
        }
    }
}
