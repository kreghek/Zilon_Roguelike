using System.Collections.Generic;

using Zilon.Core.Graphs;
using Zilon.Core.Tactics;

namespace Zilon.Bot.Players.Strategies
{
    public sealed class LogicTreeStrategyData : ILogicStrategyData
    {
        public LogicTreeStrategyData()
        {
            ObserverdNodes = new HashSet<IGraphNode>();
            ExitNodes = new HashSet<IGraphNode>();
        }

        /// <inheritdoc/>
        public HashSet<IGraphNode> ObserverdNodes { get; }

        /// <inheritdoc/>
        public HashSet<IGraphNode> ExitNodes { get; }

        /// <inheritdoc/>
        public IActor TriggerIntuder { get; set; }
    }
}