using System.Collections.Generic;

using Zilon.Core.Graphs;
using Zilon.Core.Tactics;

namespace Zilon.Bot.Players
{
    public interface ILogicStrategyData
    {
        HashSet<IGraphNode> ExitNodes { get; }

        HashSet<IGraphNode> ObserverdNodes { get; }

        IActor TriggerIntuder { get; set; }
    }
}