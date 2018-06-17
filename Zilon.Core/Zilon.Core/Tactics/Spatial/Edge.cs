namespace Zilon.Core.Tactics.Spatial
{
    public class Edge : IEdge
    {
        public IMapNode[] Nodes { get; }

        public Edge(params IMapNode[] nodes)
        {
            Nodes = nodes;
        }
    }
}
