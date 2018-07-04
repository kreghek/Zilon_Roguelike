namespace Zilon.Core.Tactics.Spatial
{
    using System.Collections.Generic;


    public class HexMap : IMap
    {
        public HexMap()
        {
            Nodes = new List<IMapNode>();
            Edges = new List<IEdge>();
        }

        public List<IMapNode> Nodes { get; set; }
        public List<IEdge> Edges { get; set; }

        public bool IsPositionAvailableFor(IMapNode targetNode, Actor actor)
        {
            return true;
        }

        public void ReleaseNode(IMapNode node, Actor actor)
        {
            // Ещё нет блокировки ячейки
        }

        public void HoldNode(IMapNode node, Actor actor)
        {
            // Ещё нет блокировки ячейки
        }
    }
}
