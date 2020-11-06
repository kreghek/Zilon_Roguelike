using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Logics
{
    public abstract class LogicStateBase : ILogicState
    {
        public bool Complete { get; protected set; }

        public abstract IActorTask GetTask(IActor actor, ISectorTaskSourceContext context, ILogicStrategyData strategyData);

        public void Reset()
        {
            Complete = false;
            ResetData();
        }

        protected abstract void ResetData();
    }
}