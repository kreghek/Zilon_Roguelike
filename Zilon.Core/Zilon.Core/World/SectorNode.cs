using Zilon.Core.Graphs;
using Zilon.Core.Tactics;

namespace Zilon.Core.World
{
    public sealed class SectorNode : IGraphNode
    {
        public ISector Sector { get; }
    }
}
