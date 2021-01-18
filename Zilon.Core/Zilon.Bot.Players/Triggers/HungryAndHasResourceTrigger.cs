using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Triggers
{
    public class HungryAndHasResourceTrigger : HazardAndHasResourceTriggerBase
    {
        protected override bool TestInner(IActor actor, ISectorTaskSourceContext context, ILogicState currentState,
            ILogicStrategyData strategyData)
        {
            var taskContext = new ActorTaskContext(context.Sector);

            return SurvivalHazardTriggerHelper.TestHazardAndResource(actor, taskContext, strategyData, SurvivalStatType.Satiety,
                ConsumeCommonRuleType.Satiety);
        }
    }
}