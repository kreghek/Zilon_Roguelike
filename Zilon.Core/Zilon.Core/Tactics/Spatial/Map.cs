namespace Zilon.Core.Tactics.Spatial
{
    using System.Collections.Generic;
    using System.Linq;


    public class Map : IMap
    {
        public List<HexNode> Nodes { get; set; }

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
