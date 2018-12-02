using System.Collections.Generic;
using System.Linq;

namespace Zilon.Core.Tactics.Spatial
{
    public class HexMap : MapBase
    {
        private readonly IList<IEdge> _edges;

        public HexMap()
        {
            _edges = new List<IEdge>();
        }

        public override IEnumerable<IMapNode> GetNext(IMapNode node)
        {
            var hexCurrent = (HexNode)node;
            var hexNodes = Nodes.Cast<HexNode>().ToArray();
            var neighbors = HexNodeHelper.GetSpatialNeighbors(hexCurrent, hexNodes);

            var currentEdges = from edge in _edges
                               where edge.Nodes.Contains(node)
                               select edge;
            var currentEdgeArray = currentEdges.ToArray();

            var actualNeighbors = new List<IMapNode>();
            foreach (var testedNeighbor in neighbors)
            {
                var edge = currentEdgeArray.SingleOrDefault(x => x.Nodes.Contains(testedNeighbor));
                if (edge == null)
                {
                    continue;
                }

                yield return testedNeighbor;
            }
        }
    }
}
