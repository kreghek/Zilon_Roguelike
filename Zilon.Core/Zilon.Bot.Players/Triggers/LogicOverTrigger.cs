using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Triggers
{
    /// <summary>
    /// Триггер, определяющий, что текущее состояние дошло до логического завершения.
    /// </summary>
    public sealed class LogicOverTrigger : ILogicStateTrigger
    {
        public void Reset()
        {
            // Нет состояния.
        }

        public bool Test(
            IActor actor,
            ISectorTaskSourceContext context,
            ILogicState currentState,
            ILogicStrategyData strategyData)
        {
            if (currentState is null)
            {
                throw new System.ArgumentNullException(nameof(currentState));
            }

            return currentState.Complete;
        }

        public void Update()
        {
            // Нет состояния.
        }
    }
}