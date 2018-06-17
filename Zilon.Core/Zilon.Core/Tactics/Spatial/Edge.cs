namespace Zilon.Core.Tactics.Spatial
{
    public class Edge : IEdge
    {
        public IMapNode[] Nodes { get; }

        public int Cost => 1;

        public Edge(params IMapNode[] nodes)
        {
            Nodes = nodes;
        }
    }
}
