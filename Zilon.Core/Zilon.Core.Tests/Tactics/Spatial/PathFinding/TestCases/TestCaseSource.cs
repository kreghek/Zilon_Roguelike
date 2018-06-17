using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.Tactics.Spatial.PathFinding.TestCases
{
    public class TestCaseSource
    {
        public static IEnumerable TestCases
        {
            get
            {
                yield return T1();
            }
        }

        private static TestCaseData T1()
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
    }
}
