using Zilon.Core.Graphs;
using Zilon.Core.Tactics;

namespace Zilon.Core.PathFinding
{
    public interface IPathFindingContext
    {
        IActor Actor { get; }

        IGraphNode TargetNode { get; set; }
    }
}
