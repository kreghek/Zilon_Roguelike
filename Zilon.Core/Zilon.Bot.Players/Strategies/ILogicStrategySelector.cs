using Zilon.Core.Tactics;

namespace Zilon.Bot.Players.Strategies
{
    public interface ILogicStrategySelector
    {
        ILogicStrategy GetLogicStrategy(IActor actor);
    }
}
