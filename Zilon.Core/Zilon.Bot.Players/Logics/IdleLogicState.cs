using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;

namespace Zilon.Bot.Players.Logics
{
    public sealed class IdleLogicState : LogicStateBase
    {
        private IdleTask IdleTask;
        private readonly IDecisionSource _decisionSource;

        public IdleLogicState(IDecisionSource decisionSource)
        {
            _decisionSource = decisionSource;
        }

        public override IActorTask GetTask(IActor actor, ILogicStrategyData strategyData)
        {
            if (IdleTask == null)
            {
                IdleTask = new IdleTask(actor, _decisionSource);
            }

            if (IdleTask.IsComplete)
            {
                Complete = true;
                return null;
            }

            return IdleTask;
        }

        protected override void ResetData()
        {
            IdleTask = null;
        }
    }
}