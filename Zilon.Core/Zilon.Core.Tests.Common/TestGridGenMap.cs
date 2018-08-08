using System.Collections.Generic;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.TestCommon
{
    using Zilon.Core.CommonServices.MapGenerators;

    public class TestGridGenMap : IMap
    {
        public List<IMapNode> Nodes { get; set; }

        public List<IEdge> Edges { get; set; }

        public void HoldNode(IMapNode node, Actor actor)
        {

        }

        public bool IsPositionAvailableFor(IMapNode targetNode, Actor actor)
        {
            return true;
        }

        public void ReleaseNode(IMapNode node, Actor actor)
        {

        }

        public TestGridGenMap()
        {
            Nodes = new List<IMapNode>();
            Edges = new List<IEdge>();

            var gridGenerator = new GridMapGenerator(10);
            gridGenerator.CreateMap(this);
        }
    }
}
