using System.Collections.Generic;
using Zilon.Core.Services.MapGenerators;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.TestCommon
{
    /// <summary>
    /// Тестовый класс карты со стенами.
    /// </summary>
    public class TestGridGenWallMap : IMap
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

        public TestGridGenWallMap()
        {
            Nodes = new List<IMapNode>();
            Edges = new List<IEdge>();

            var gridGenerator = new GridMapGenerator(10);
            gridGenerator.CreateMap(this);
        }
    }
}
