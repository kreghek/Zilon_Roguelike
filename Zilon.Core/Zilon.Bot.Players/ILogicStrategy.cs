using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players
{
    public interface ILogicStrategy
    {
        IActor Actor { get; }

        ILogicState CurrentState { get; }

        IActorTask GetActorTask();

        ILogicStrategyData StrategyData { get; }
    }
}