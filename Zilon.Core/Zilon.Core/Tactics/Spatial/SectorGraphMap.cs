using System.Collections.Generic;
using Zilon.Core.Graphs;

namespace Zilon.Core.Tactics.Spatial
{
    public sealed class SectorGraphMap : GraphMap, ISectorMap
    {
        public Dictionary<IGraphNode, SectorTransition> Transitions { get; }

        public SectorGraphMap()
        {
            Transitions = new Dictionary<IGraphNode, SectorTransition>();
        }

        public int DistanceBetween(IGraphNode currentNode, IGraphNode targetNode)
        {
            return 0;
        }
    }
}
