using System.Linq;

using Zilon.Bot.Players.Triggers;
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
            var bestResource = ResourceFinder.FindBestConsumableResourceByRule(resources,
                resourceType);

            if (bestResource == null)
            {
                return null;
            }

            return new UsePropTask(actor, bestResource);
        }
    }
}
