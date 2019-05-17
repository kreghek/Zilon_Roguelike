using System.Linq;

using Zilon.Bot.Players.Triggers;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Logics
{
    public class HealSelfLogicState : ILogicState
    {
        public bool Complete { get; private set; }

        public ILogicStateData CreateData(IActor actor)
        {
            return new EmptyLogicTriggerData();
        }

        public IActorTask GetTask(IActor actor, ILogicStateData data)
        {
            var hpStat = actor.Person.Survival.Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Health);
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
    }
}
