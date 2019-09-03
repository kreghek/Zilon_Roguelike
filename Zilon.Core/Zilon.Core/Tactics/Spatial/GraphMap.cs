using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using JetBrains.Annotations;

namespace Zilon.Core.Tactics.Spatial
{
    public class GraphMap : MapBase
    {
        private readonly IList<IMapNode> _nodes;
        private readonly IList<IEdge> _edges;

        [ExcludeFromCodeCoverage]
        public GraphMap()
        {
            _edges = new List<IEdge>();
            _nodes = new List<IMapNode>();
        }

        public override IEnumerable<IMapNode> Nodes { get => _nodes; }

        public override void AddEdge([NotNull] IMapNode node1, [NotNull]  IMapNode node2)
        {
            if (node1 == null)
            {
                throw new ArgumentNullException(nameof(node1));
            }

            if (node2 == null)
            {
                throw new ArgumentNullException(nameof(node2));
            }

            if (!CheckNodeInMap(node1))
            {
                throw new ArgumentException($"Указанный узел {node1} не найден в текущей карте", nameof(node1));
            }

            if (!CheckNodeInMap(node2))
            {
                throw new ArgumentException($"Указанный узел {node2} не найден в текущей карте", nameof(node2));
            }

            _edges.Add(new Edge(node1, node2));
        }

        [ExcludeFromCodeCoverage]
        public override void AddNode(IMapNode node)
        {
            _nodes.Add(node);
        }

        public bool TargetIsOnLine(IMapNode currentNode, IMapNode targetNode)
        {
            return true;
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

        private bool CheckNodeInMap(IMapNode node)
        {
            return Nodes.Contains(node);
        }

        public override bool IsPositionAvailableForContainer(IMapNode targetNode)
        {
            throw new NotImplementedException();
        }
    }
}
