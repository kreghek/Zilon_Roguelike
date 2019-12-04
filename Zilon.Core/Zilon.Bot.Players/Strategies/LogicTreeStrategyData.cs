using System.Collections.Generic;
using Zilon.Core.Graphs;

namespace Zilon.Bot.Players.Strategies
{
    public sealed class LogicTreeStrategyData : ILogicStrategyData
    {
        public LogicTreeStrategyData()
        {
            ObserverdNodes = new HashSet<IGraphNode>();
            ExitNodes = new HashSet<IGraphNode>();
        }

        public HashSet<IGraphNode> ObserverdNodes { get; }
        public HashSet<IGraphNode> ExitNodes { get; }
    }
}
