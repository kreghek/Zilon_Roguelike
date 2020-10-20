using Zilon.Core.Tactics;

namespace Zilon.Bot.Players
{
    public interface ILogicStateTrigger
    {
        bool Test(IActor actor, ILogicState currentState, ILogicStrategyData strategyData);

        void Update();

        void Reset();
    }
}