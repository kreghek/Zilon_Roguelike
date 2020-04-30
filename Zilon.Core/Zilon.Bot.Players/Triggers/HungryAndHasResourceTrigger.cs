using System.Linq;

using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Bot.Players.Triggers
{
    public class HungryAndHasResourceTrigger : ILogicStateTrigger
    {
        public void Reset()
        {
            // Нет состояния.
        }

        public bool Test(IActor actor, ILogicState currentState, ILogicStrategyData strategyData)
        {
            if (actor is null)
            {
                throw new System.ArgumentNullException(nameof(actor));
            }

            var hazardEffect = actor.Person.GetModule<IEffectsModule>().Items.OfType<SurvivalStatHazardEffect>()
                .SingleOrDefault(x => x.Type == SurvivalStatType.Satiety);
            if (hazardEffect == null)
            {
                return false;
            }

            //

            var props = actor.Person.GetModule<IInventoryModule>().CalcActualItems();
            var resources = props.OfType<Resource>();
            var bestResource = ResourceFinder.FindBestConsumableResourceByRule(resources,
                ConsumeCommonRuleType.Satiety);

            if (bestResource == null)
            {
                return false;
            }

            return true;
        }

        public void Update()
        {
            // нет состояния.
        }
    }
}
