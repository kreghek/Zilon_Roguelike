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
#pragma warning disable IDE0016 // Use 'throw' expression
            if (nodes == null)
            {
                throw new ArgumentNullException(nameof(nodes), "Не указаны узлы, соединённые ребром.");
            }
#pragma warning restore IDE0016 // Use 'throw' expression

            Nodes = nodes;
        }
    }
}
