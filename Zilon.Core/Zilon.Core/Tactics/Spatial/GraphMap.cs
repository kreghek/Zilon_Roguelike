using Zilon.Core.Graphs;

namespace Zilon.Core.Tactics.Spatial
{
    public class GraphMap : MapBase
    {
        private readonly IList<IGraphEdge> _edges;
        private readonly IList<IGraphNode> _nodes;

        [ExcludeFromCodeCoverage]
        public GraphMap()
        {
            _edges = new List<IGraphEdge>();
            _nodes = new List<IGraphNode>();
        }

        public override IEnumerable<IGraphNode> Nodes => _nodes;

        public override void AddEdge([NotNull] IGraphNode node1, [NotNull] IGraphNode node2)
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
        public override void AddNode(IGraphNode node)
        {
            _nodes.Add(node);
        }

        public bool TargetIsOnLine(IGraphNode currentNode, IGraphNode targetNode)
        {
            return true;
        }

        public override IEnumerable<IGraphNode> GetNext(IGraphNode node)
        {
            HexNode hexCurrent = (HexNode)node;
            var hexNodes = Nodes.Cast<HexNode>().ToArray();
            HexNode[] neighbors = HexNodeHelper.GetSpatialNeighbors(hexCurrent, hexNodes);

            var currentEdges = from edge in _edges
                where edge.Nodes.Contains(node)
                select edge;
            var currentEdgeArray = currentEdges.ToArray();

            foreach (HexNode testedNeighbor in neighbors)
            {
                var edge = currentEdgeArray.SingleOrDefault(x => x.Nodes.Contains(testedNeighbor));
                if (edge == null)
                {
                    continue;
                }

                yield return testedNeighbor;
            }
        }

        public override void RemoveEdge(IGraphNode node1, IGraphNode node2)
        {
            var currentEdge = (from edge in _edges
                where edge.Nodes.Contains(node1)
                where edge.Nodes.Contains(node2)
                select edge).Single();

            _edges.Remove(currentEdge);
        }

        private bool CheckNodeInMap(IGraphNode node)
        {
            return Nodes.Contains(node);
        }

        public override bool IsPositionAvailableForContainer(IGraphNode targetNode)
        {
            throw new NotImplementedException();
        }

        public override int DistanceBetween(IGraphNode currentNode, IGraphNode targetNode)
        {
            if (currentNode is null)
            {
                throw new ArgumentNullException(nameof(currentNode));
            }

            if (targetNode is null)
            {
                throw new ArgumentNullException(nameof(targetNode));
            }

            if (!(currentNode is HexNode))
            {
                throw new ArgumentException($"{nameof(currentNode)} должен быть типа {typeof(HexNode)}");
            }

            if (!(targetNode is HexNode))
            {
                throw new ArgumentException($"{nameof(targetNode)} должен быть типа {typeof(HexNode)}");
            }

            HexNode actorHexNode = (HexNode)currentNode;
            HexNode containerHexNode = (HexNode)targetNode;

            CubeCoords actorCoords = actorHexNode.CubeCoords;
            CubeCoords containerCoords = containerHexNode.CubeCoords;

            var distance = actorCoords.DistanceTo(containerCoords);

            return distance;
        }

        /// <inheritdoc />
        public override void RemoveNode(IGraphNode node)
        {
            _nodes.Remove(node);

            var edgesCopy = _edges.ToArray();
            foreach (var edge in edgesCopy)
            {
                if (edge.Nodes.Contains(node))
                {
                    _edges.Remove(edge);
                }
            }
        }
    }
}