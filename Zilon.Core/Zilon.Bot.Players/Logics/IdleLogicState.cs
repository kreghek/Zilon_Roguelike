using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;

namespace Zilon.Bot.Players.Logics
{
    public sealed class IdleLogicState : ILogicState
    {
        private readonly IDecisionSource _decisionSource;

        public IdleLogicState(IDecisionSource decisionSource)
        {
            _decisionSource = decisionSource;
        }

        public bool Complete { get; private set; }

        public ILogicStateData CreateData(IActor actor)
        {
            return new IdleLogicData();
        }

        public IActorTask GetTask(IActor actor, ILogicStateData data)
        {
            var logicData = (IdleLogicData)data;
            if (logicData.IdleTask == null)
            {
                logicData.IdleTask = new IdleTask(actor, _decisionSource);
            }

            if (logicData.IdleTask.IsComplete)
            {
                Complete = true;
                return null;
            }

            return logicData.IdleTask;
        }
    }
}
