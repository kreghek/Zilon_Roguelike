using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;

namespace Zilon.Bot.Players.Logics
{
    public sealed class IdleLogicState : LogicStateBase
    {
        private IdleTask _idleTask;
        private readonly IDecisionSource _decisionSource;

        public IdleLogicState(IDecisionSource decisionSource)
        {
            _decisionSource = decisionSource;
        }

        public override IActorTask GetTask(IActor actor, ISectorTaskSourceContext context, ILogicStrategyData strategyData)
        {
            if (_idleTask == null)
            {
                var taskContext = new ActorTaskContext(context.Sector);
                _idleTask = new IdleTask(actor, taskContext, _decisionSource);
            }

            if (_idleTask.IsComplete)
            {
                Complete = true;
                return null;
            }

            return _idleTask;
        }

        protected override void ResetData()
        {
            _idleTask = null;
        }
    }
}