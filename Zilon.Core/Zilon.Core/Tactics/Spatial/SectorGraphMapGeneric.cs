using System.Collections.Generic;

using Zilon.Core.MapGenerators;

namespace Zilon.Core.Tactics.Spatial
{
    public class SectorGraphMap<TNode, TNodeDistanceCalculator>: GraphMap, ISectorMap
        where TNode: IMapNode
        where TNodeDistanceCalculator: IMapNodeDistanceCalculator<TNode>, new()
    {
        private readonly TNodeDistanceCalculator _nodeDistanceCalculator;

        public Dictionary<IMapNode, RoomTransition> Transitions { get; }

        public SectorGraphMap()
        {
            _nodeDistanceCalculator = new TNodeDistanceCalculator();

            Transitions = new Dictionary<IMapNode, RoomTransition>();
        }

        public int DistanceBetween(IMapNode currentNode, IMapNode targetNode)
        {
            var distance = _nodeDistanceCalculator.GetDistance((TNode)currentNode, (TNode)targetNode);
            return distance;
        }
    }
}
