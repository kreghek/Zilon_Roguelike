using System.Collections.Generic;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.TestCommon
{
    public class TestMap : IMap
    {
        public List<HexNode> Nodes { get; set; }

        public void HoldNode(HexNode node, Actor actor)
        {

        }

        public bool IsPositionAvailableFor(HexNode targetNode, Actor actor)
        {
            return true;
        }

        public void ReleaseNode(HexNode node, Actor actor)
        {

        }

        public TestMap()
        {
            Nodes = new List<HexNode>();
        }
    }
}
