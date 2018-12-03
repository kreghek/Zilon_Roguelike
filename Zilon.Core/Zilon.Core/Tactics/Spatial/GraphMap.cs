using System.Collections.Generic;
using System.Linq;

namespace Zilon.Core.Tactics.Spatial
{
    public class GraphMap : MapBase
    {
        private readonly IList<IMapNode> _nodes;
        private readonly IList<IEdge> _edges;

        public GraphMap()
        {
            _edges = new List<IEdge>();
            _nodes = new List<IMapNode>();
        }

        public override IEnumerable<IMapNode> Nodes { get => _nodes; }

        public override void AddEdge(IMapNode node1, IMapNode node2)
        {
            _edges.Add(new Edge(node1, node2));
        }

        public override void AddNode(IMapNode node)
        {
            _nodes.Add(node);
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

        public override void RemoveEdge(IMapNode node1, IMapNode node2)
        {
            var currentEdge = (from edge in _edges
                               where edge.Nodes.Contains(node1)
                               where edge.Nodes.Contains(node2)
                               select edge).Single();

            _edges.Remove(currentEdge);
        }
    }
}
