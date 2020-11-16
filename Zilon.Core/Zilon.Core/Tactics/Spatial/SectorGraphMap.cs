using System.Collections.Generic;

using Zilon.Core.Graphs;
using Zilon.Core.MapGenerators;

namespace Zilon.Core.Tactics.Spatial
{
    public sealed class SectorGraphMap : GraphMap, ISectorMap
    {
        public SectorGraphMap()
        {
            Transitions = new Dictionary<IGraphNode, RoomTransition>();
        }

        public Dictionary<IGraphNode, RoomTransition> Transitions { get; }

        public override int DistanceBetween(IGraphNode currentNode, IGraphNode targetNode)
        {
            //TODO Жуткий костыль. Перепиать код, чтобы его не было. Тесты должны проходить.
            if (currentNode is HexNode hexCurrentNode && targetNode is HexNode hexTargetNode)
            {
                return hexCurrentNode.CubeCoords.DistanceTo(hexTargetNode.CubeCoords);
            }

            return 0;
        }
    }
}