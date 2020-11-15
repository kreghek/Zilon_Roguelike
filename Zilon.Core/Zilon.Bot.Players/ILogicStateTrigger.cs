using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players
{
    public interface ILogicStateTrigger
    {
        void Reset();

        bool Test(
            IActor actor,
            ISectorTaskSourceContext context,
            ILogicState currentState,
            ILogicStrategyData strategyData);

        void Update();
    }
}