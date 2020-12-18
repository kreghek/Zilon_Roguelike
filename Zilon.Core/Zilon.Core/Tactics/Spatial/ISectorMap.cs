using System.Collections.Generic;

using Zilon.Core.Graphs;
using Zilon.Core.MapGenerators;

namespace Zilon.Core.Tactics.Spatial
{
    public interface ISectorMap : IMap
    {
        /// <summary>
        /// Identifier of map.
        /// </summary>
        /// <remarks>
        /// Can be used in tests.
        /// </remarks>
        int Id { get; set; }

        /// <summary>
        /// Transitions and transition nodes between sector levels.
        /// </summary>
        Dictionary<IGraphNode, RoomTransition> Transitions { get; }

        /// <summary>
        /// Check availability of the target node from the start node using line.
        /// </summary>
        /// <param name="currentNode">Start node.</param>
        /// <param name="targetNode">Target node.</param>
        /// <returns> Returns <c>true</c> if <see cref="targetNode"/> is on line from
        /// <see cref="currentNode"/>. <c>false</c> - otherwise.</returns>
        bool TargetIsOnLine(IGraphNode currentNode, IGraphNode targetNode);
    }
}