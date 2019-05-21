using System.Collections.Generic;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players.Strategies
{
    public sealed class LogicTreeStrategyData : ILogicStrategyData
    {
        public LogicTreeStrategyData()
        {
            ObserverdNodes = new HashSet<IMapNode>();
            ExitNodes = new HashSet<IMapNode>();
        }

        public HashSet<IMapNode> ObserverdNodes { get; }
        public HashSet<IMapNode> ExitNodes { get; }
    }
}
