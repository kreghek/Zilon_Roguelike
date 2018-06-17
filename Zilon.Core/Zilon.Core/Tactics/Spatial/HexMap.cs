namespace Zilon.Core.Tactics.Spatial
{
    using System.Collections.Generic;


    public class HexMap : IMap<HexNode, Edge>
    {
        public List<HexNode> Nodes { get; set; }
        public List<Edge> Edges { get; set; }

        public bool IsPositionAvailableFor(HexNode targetNode, Actor actor)
        {
            return true;
        }

        public void ReleaseNode(HexNode node, Actor actor)
        {
            
        }

        public void HoldNode(HexNode node, Actor actor)
        {
            
        }
    }
}
