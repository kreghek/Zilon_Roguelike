using Zilon.Core.Graphs;

namespace Zilon.Core.Tactics.Spatial
{
    /// <summary>
    /// Базовая реализация ребра между двумя узлами.
    /// </summary>
    public class Edge : IGraphEdge
    {
        public Edge(params IGraphNode[] nodes)
        {
            Nodes = nodes ?? throw new ArgumentNullException(nameof(nodes), "Не указаны узлы, соединённые ребром.");
        }

        public IGraphNode[] Nodes { get; }

        public int Cost => 1;
    }
}