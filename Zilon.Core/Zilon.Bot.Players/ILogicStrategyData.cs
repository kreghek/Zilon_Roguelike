using System.Collections.Generic;

using Zilon.Core.Graphs;
using Zilon.Core.Props;
using Zilon.Core.Tactics;

namespace Zilon.Bot.Players
{
    public interface ILogicStrategyData
    {
        /// <summary>
        /// Transition nodes which was found during exploration of sector.
        /// </summary>
        HashSet<IGraphNode> ExitNodes { get; }

        /// <summary>
        /// Known nodes for actors without FoW data module.
        /// </summary>
        HashSet<IGraphNode> ObservedNodes { get; }

        /// <summary>
        /// Resource which was selected by trigger witch test survival hazard and ability to reduce it.
        /// </summary>
        Resource ResourceToReduceHazard { get; set; }

        Equipment TargetEquipment { get; set; }
        int? TargetEquipmentSlot { get; set; }

        /// <summary>
        /// Intruder which was detected in trigger.
        /// </summary>
        IActor TriggerIntuder { get; set; }
    }
}