using System;
using System.Collections.Generic;
using System.Linq;

namespace Zilon.Core.Tactics.Spatial
{
    public class GraphMap : MapBase
    {
        private readonly IList<IEdge> _edges;

        public GraphMap()
        {
            _edges = new List<IEdge>();
        }

        public override IEnumerable<IMapNode> Nodes { get; }

        public override void AddEdge(IMapNode node1, IMapNode node2)
        {
            _edges.Add(new Edge(node1, node2));
        }

        public override void AddNode(IMapNode node)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<IMapNode> GetNext(IMapNode node)
        {
            throw new NotImplementedException();
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
