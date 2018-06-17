using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Moq;

using NUnit.Framework;

using Zilon.Core.Services.MapGenerators;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.TestCommon;

namespace Zilon.Core.Tests.Tactics.Spatial.PathFinding.TestCases
{
    public static class HexPathTestCaseSource
    {
        public static IEnumerable HexTestCases
        {
            get
            {
                yield return SingleWall();
                yield return EmptyGrid_LinePath();
            }
        }

        private static TestCaseData SingleWall()
        {
            var nodes = new List<IMapNode>();
            var edges = new List<IEdge>();
            var path = new List<IMapNode>();

            nodes.Add(new HexNode(0, 0));
            nodes.Add(new HexNode(1, 0));
            nodes.Add(new HexNode(0, 1));

            edges.Add(new Edge(nodes[0], nodes[2]));
            edges.Add(new Edge(nodes[2], nodes[1]));

            path.Add(nodes[0]);
            path.Add(nodes[2]);
            path.Add(nodes[1]);

            return new TestCaseData(nodes, edges, path.ToArray());
        }

        private static TestCaseData EmptyGrid_LinePath() {

            var mapMock = new Mock<IMap>();

            var nodes = new List<IMapNode>();
            var edges = new List<IEdge>();
            mapMock.SetupProperty(x => x.Nodes, nodes);
            mapMock.SetupProperty(x => x.Edges, edges);

            var map = mapMock.Object;

            var genertor = new GridMapGenerator();
            genertor.CreateMap(map);

            var path = new IMapNode[] {
                nodes.Cast<HexNode>().SelectBy(1,1),
                nodes.Cast<HexNode>().SelectBy(2,2),
                nodes.Cast<HexNode>().SelectBy(2,3),
                nodes.Cast<HexNode>().SelectBy(3,4),
                nodes.Cast<HexNode>().SelectBy(3,5),
                nodes.Cast<HexNode>().SelectBy(4,6)
            };

            return new TestCaseData(nodes, edges, path);
        }
    }
}
