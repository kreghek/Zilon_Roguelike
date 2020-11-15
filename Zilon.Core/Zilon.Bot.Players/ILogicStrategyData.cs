using Zilon.Core.Graphs;

namespace Zilon.Bot.Players
{
    public interface ILogicStrategyData
    {
        HashSet<IGraphNode> ExitNodes { get; }

        HashSet<IGraphNode> ObserverdNodes { get; }
    }
}