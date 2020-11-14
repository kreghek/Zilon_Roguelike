using Zilon.Core.Graphs;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Benchmarks.Fow
{
    internal class TestFowContext : IFowContext
    {
        private readonly ISectorMap _sectorMap;

        public TestFowContext(ISectorMap sectorMap)
        {
            _sectorMap = sectorMap ?? throw new ArgumentNullException(nameof(sectorMap));
        }

        public IEnumerable<IGraphNode> GetNext(IGraphNode node)
        {
            return _sectorMap.GetNext(node);
        }

        public bool IsTargetVisible(IGraphNode baseNode, IGraphNode targetNode)
        {
            return _sectorMap.TargetIsOnLine(baseNode, targetNode);
        }
    }
}