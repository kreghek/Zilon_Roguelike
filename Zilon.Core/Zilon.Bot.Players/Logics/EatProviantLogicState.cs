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
        public override IActorTask GetTask(IActor actor, ISectorTaskSourceContext context, ILogicStrategyData strategyData)
        {
            if (actor is null)
            {
                throw new System.ArgumentNullException(nameof(actor));
            }

            if (context is null)
            {
                throw new System.ArgumentNullException(nameof(context));
            }

            var eatFoodTask = CheckHazard(actor, context, SurvivalStatType.Satiety, ConsumeCommonRuleType.Satiety);
            if (eatFoodTask != null)
            {
                return eatFoodTask;
            }

            var drinkWaterTask = CheckHazard(actor, context, SurvivalStatType.Hydration, ConsumeCommonRuleType.Thirst);
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

        private UsePropTask CheckHazard(IActor actor, ISectorTaskSourceContext context, SurvivalStatType hazardType, ConsumeCommonRuleType resourceType)
        {
            var hazardEffect = actor.Person.GetModule<IEffectsModule>().Items.OfType<SurvivalStatHazardEffect>()
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

            var taskContxt = new ActorTaskContext(context.Sector, context.Globe);
            return new UsePropTask(actor, taskContxt, bestResource);
        }
    }
}