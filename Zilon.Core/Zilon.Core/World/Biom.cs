using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Graphs;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.World
{
    /// <summary>
    /// Биом, как совокупность нескольких секторов.
    /// </summary>
    public sealed class Biom : IGraph
    {
        private readonly IList<SectorNode> _nodes;
        private readonly IList<IGraphEdge> _edges;

        public Biom()
        {
            _nodes = new List<SectorNode>();
            _edges = new List<IGraphEdge>();
        }

        public IEnumerable<SectorNode> Sectors { get; }

        public IEnumerable<IGraphNode> Nodes { get => _nodes; }

        public void AddEdge(IGraphNode node1, IGraphNode node2)
        {
            var edge = new Edge(node1, node2);
            _edges.Add(edge);
        }

        public void AddNode(IGraphNode node)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var sectorNode = node as SectorNode;
            if (sectorNode is null)
            {
                throw new ArgumentException("Узел должен быть сектором", nameof(node));
            }

            _nodes.Add(sectorNode);
        }

        public IEnumerable<IGraphNode> GetNext(IGraphNode node)
        {
            // Алгоритм (наивный).
            // Выбираем все ребра, в которых упоминается указанный узел.
            // Выбираем все другие узлы, которые указаны в выбранных ребрах.

            var currentEdges = from edge in _edges
                               where edge.Nodes.Contains(node)
                               select edge;

            var currentEdgeArray = currentEdges.ToArray();

            return currentEdgeArray.SelectMany(x => x.Nodes).Where(x => x != node);
        }

        public void RemoveEdge(IGraphNode node1, IGraphNode node2)
        {
            // Эта операция запрещает использование дублирующихся рёбер.
            // Для рёбер может быть только одна комбинация соединённых узлов.

            var edge = _edges.SingleOrDefault(x => x.Nodes.Contains(node1) && x.Nodes.Contains(node2));

            if (edge == null)
            {
                throw new ArgumentException($"Указаны узлы ({node1}, {node2}), которые не соединены ребром.");
            }

            _edges.Remove(edge);
        }

        public void RemoveNode(IGraphNode node)
        {
            _nodes.Remove(node);
        }
    }
}
