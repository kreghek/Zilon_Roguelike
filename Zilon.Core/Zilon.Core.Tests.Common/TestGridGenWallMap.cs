using System.Collections.Generic;
using System.Linq;
using Zilon.Core.MapGenerators;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.Common
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

            RemoveEdge(3, 3, 3, 4);
            RemoveEdge(3, 3, 2, 3);
        }

        private IEdge GetEdge(int offsetX1, int offsetY1, int offsetX2, int offsetY2)
        {
            var foundFromStart = from edge in Edges
                                 from node in edge.Nodes
                                 let hexNode = (HexNode)node
                                 where hexNode.OffsetX == offsetX1 && hexNode.OffsetY == offsetY1
                                 select edge;

            var foundToEnd = from edge in foundFromStart
                             from node in edge.Nodes
                             let hexNode = (HexNode)node
                             where hexNode.OffsetX == offsetX2 && hexNode.OffsetY == offsetY2
                             select edge;

            return foundToEnd.SingleOrDefault();
        }

        private void RemoveEdge(int offsetX1, int offsetY1, int offsetX2, int offsetY2)
        {
            var edge = GetEdge(offsetX1, offsetY1, offsetX2, offsetY2);
            Edges.Remove(edge);
        }
    }
}
