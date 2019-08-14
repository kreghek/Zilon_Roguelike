using Zilon.Core.Tactics;

namespace Zilon.Bot.Players.Triggers
{
    public sealed class LogicOverTrigger : ILogicStateTrigger
    {
        public void Reset()
        {
            // Нет состояния.
        }

        public bool Test(IActor actor, ILogicState currentState, ILogicStrategyData strategyData)
        {
            return currentState.Complete;
        }

        public void Update()
        {
            // Нет состояния.
        }
    }
}
