using System.Collections.Generic;
using Zilon.Core.Services.MapGenerators;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.TestCommon
{
    public class TestGrid15GenMap : IMap
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

        public TestGrid15GenMap()
        {
            Nodes = new List<IMapNode>();
            Edges = new List<IEdge>();

            var gridGenerator = new GridMapGenerator(15);
            gridGenerator.CreateMap(this);
        }
    }
}
