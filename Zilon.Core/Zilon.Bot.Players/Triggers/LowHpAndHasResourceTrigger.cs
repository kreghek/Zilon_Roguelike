using System.Linq;

using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Triggers
{
    public class LowHpAndHasResourceTrigger : ILogicStateTrigger
    {
        public void Reset()
        {
            // Нет состояния
        }

        public bool Test(IActor actor, ISectorTaskSourceContext context, ILogicState currentState, ILogicStrategyData strategyData)
        {
            if (actor is null)
            {
                throw new System.ArgumentNullException(nameof(actor));
            }

            if (currentState is null)
            {
                throw new System.ArgumentNullException(nameof(currentState));
            }

            if (strategyData is null)
            {
                throw new System.ArgumentNullException(nameof(strategyData));
            }

            //TODO Здесь лучше проверять на наличие эффекта раны
            var hpStat = actor.Person.GetModule<ISurvivalModule>().Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Health);
            var isLowHp = hpStat.ValueShare <= 0.5f;
            if (!isLowHp)
            {
                return false;
            }

            //

            var props = actor.Person.GetModule<IInventoryModule>().CalcActualItems();
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