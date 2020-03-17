using System.Collections.Generic;
using Zilon.Core.Graphs;

namespace Zilon.Bot.Players
{
    public interface ILogicStrategyData
    {
        HashSet<IGraphNode> ObserverdNodes { get; }

        HashSet<IGraphNode> ExitNodes { get; }
    }
}
