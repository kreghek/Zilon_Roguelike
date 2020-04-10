using System.Linq;

using Zilon.Bot.Players.Triggers;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Logics
{
    public class HealSelfLogicState : LogicStateBase
    {
        public override IActorTask GetTask(IActor actor, ILogicStrategyData strategyData)
        {
            if (actor is null)
            {
                throw new System.ArgumentNullException(nameof(actor));
            }

            var hpStat = actor.Person.Survival.Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Health);
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

            var props = actor.Person.Inventory.CalcActualItems();
            var resources = props.OfType<Resource>();
            var bestResource = ResourceFinder.FindBestConsumableResourceByRule(resources,
                ConsumeCommonRuleType.Health);

            if (bestResource == null)
            {
                Complete = true;
                return null;
            }

            return new UsePropTask(actor, bestResource);
        }

        protected override void ResetData()
        {
            // Внутреннего состояния нет.
        }
    }
}
