using System.Collections;
using System.Linq;

using Moq;

using NUnit.Framework;

using Zilon.Core.Graphs;
using Zilon.Core.Tactics;

namespace Zilon.Bot.Players.Tests.Triggers
{
    public static class ExploredTriggerTestCaseSource
    {
        public static IEnumerable FowModuleTestCases
        {
            get {
                yield return CreateOneNodeExploredMap();
                yield return CreateTwoNodesAndNotAllExplored();
            }
        }

        /// <summary>
        /// If all nodes of map are known by fow module then the trigger returns true.
        /// </summary>
        private static TestCaseData CreateOneNodeExploredMap()
        {
            var mapNodes = new[] { Mock.Of<IGraphNode>() };
            var fowNodes = mapNodes.Select(node => new SectorMapFowNode(node)).ToArray();

            return new TestCaseData(mapNodes, fowNodes).Returns(true);
        }

        /// <summary>
        /// If not all nodes of map are known by fow module then the trigger returns false (there are nodes of map which not explored yet).
        /// </summary>
        private static TestCaseData CreateTwoNodesAndNotAllExplored()
        {
            var mapNodes = new[] { Mock.Of<IGraphNode>(), Mock.Of<IGraphNode>() };
            var fowNodes = mapNodes.Skip(1).Select(node => new SectorMapFowNode(node)).ToArray();

            return new TestCaseData(mapNodes, fowNodes).Returns(false);
        }
    }
}
