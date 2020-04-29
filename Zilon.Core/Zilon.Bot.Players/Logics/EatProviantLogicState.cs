using System.Linq;

using Zilon.Bot.Players.Triggers;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Logics
{
    public class EatProviantLogicState : LogicStateBase
    {
        public override IActorTask GetTask(IActor actor, ILogicStrategyData strategyData)
        {
            if (actor is null)
            {
                throw new System.ArgumentNullException(nameof(actor));
            }

            var eatFoodTask = CheckHazard(actor, SurvivalStatType.Satiety, ConsumeCommonRuleType.Satiety);
            if (eatFoodTask != null)
            {
                return eatFoodTask;
            }

            var drinkWaterTask = CheckHazard(actor, SurvivalStatType.Hydration, ConsumeCommonRuleType.Thirst);
            if (drinkWaterTask != null)
            {
                return drinkWaterTask;
            }

            Complete = true;
            return null;
        }

        protected override void ResetData()
        {
            // Внутреннего состояния нет.
        }

        private UsePropTask CheckHazard(IActor actor, SurvivalStatType hazardType, ConsumeCommonRuleType resourceType)
        {
            var hazardEffect = actor.Person.Effects.Items.OfType<SurvivalStatHazardEffect>()
                .SingleOrDefault(x => x.Type == hazardType);
            if (hazardEffect == null)
            {
                return null;
            }

            var props = actor.Person.GetModule<IInventoryModule>().CalcActualItems();
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
