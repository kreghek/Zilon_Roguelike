using System.Collections.Generic;

using Zilon.Core.Graphs;
using Zilon.Core.Props;
using Zilon.Core.Tactics;

namespace Zilon.Bot.Players
{
    public interface ILogicStrategyData
    {
        HashSet<IGraphNode> ExitNodes { get; }

        HashSet<IGraphNode> ObserverdNodes { get; }

        Resource ResourceToReduceHazard { get; set; }

        IActor TriggerIntuder { get; set; }
    }
}