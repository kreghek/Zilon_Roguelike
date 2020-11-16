using System.Collections.Generic;

using Zilon.Core.Graphs;
using Zilon.Core.MapGenerators;

namespace Zilon.Core.Tactics.Spatial
{
    public class SectorGraphMap<TNode, TNodeDistanceCalculator> : GraphMap, ISectorMap
        where TNode : IGraphNode
        where TNodeDistanceCalculator : IMapNodeDistanceCalculator<TNode>, new()
    {
        private readonly TNodeDistanceCalculator _nodeDistanceCalculator;

        public SectorGraphMap()
        {
            _nodeDistanceCalculator = new TNodeDistanceCalculator();

            Transitions = new Dictionary<IGraphNode, RoomTransition>();
        }

        public Dictionary<IGraphNode, RoomTransition> Transitions { get; }

        public override int DistanceBetween(IGraphNode currentNode, IGraphNode targetNode)
        {
            var distance = _nodeDistanceCalculator.GetDistance((TNode)currentNode, (TNode)targetNode);
            return distance;
        }
    }
}