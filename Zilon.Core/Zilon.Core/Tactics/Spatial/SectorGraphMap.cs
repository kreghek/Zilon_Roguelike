using System.Collections.Generic;
using Zilon.Core.Graphs;
using Zilon.Core.MapGenerators;

namespace Zilon.Core.Tactics.Spatial
{
    public sealed class SectorGraphMap : GraphMap, ISectorMap
    {
        public Dictionary<IGraphNode, RoomTransition> Transitions { get; }

        public SectorGraphMap()
        {
            Transitions = new Dictionary<IGraphNode, RoomTransition>();
        }

        public override int DistanceBetween(IGraphNode currentNode, IGraphNode targetNode)
        {
            return 0;
        }
    }
}
