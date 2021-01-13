using System.Collections.Generic;

using Zilon.Core.Graphs;
using Zilon.Core.Props;
using Zilon.Core.Tactics;

namespace Zilon.Bot.Players.Strategies
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed class LogicTreeStrategyData : ILogicStrategyData
    {
        public LogicTreeStrategyData()
        {
            ObservedNodes = new HashSet<IGraphNode>();
            ExitNodes = new HashSet<IGraphNode>();
        }

        /// <inheritdoc />
        public HashSet<IGraphNode> ObservedNodes { get; }

        /// <inheritdoc />
        public HashSet<IGraphNode> ExitNodes { get; }

        /// <inheritdoc />
        public IActor TriggerIntuder { get; set; }

        /// <inheritdoc />
        public Resource ResourceToReduceHazard { get; set; }

        /// <inheritdoc />
        public Equipment TargetEquipment { get; set; }

        /// <inheritdoc />
        public int? TargetEquipmentSlotIndex { get; set; }
    }
}