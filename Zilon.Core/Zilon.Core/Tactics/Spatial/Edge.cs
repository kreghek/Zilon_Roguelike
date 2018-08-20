using System;

namespace Zilon.Core.Tactics.Spatial
{
    /// <summary>
    /// Базовая реализация ребра между двумя узлами.
    /// </summary>
    public class Edge : IEdge
    {
        public IMapNode[] Nodes { get; }

        public int Cost => 1;

        public Edge(params IMapNode[] nodes)
        {
            Nodes = nodes ?? throw new ArgumentNullException(nameof(nodes), "Не указаны узлы, соединённые ребром.");
        }
    }
}
