using Zilon.Core.Graphs;

namespace Zilon.Core.Tactics.Spatial.PathFinding
{
    public interface IPathFindingContext
    {
        IActor Actor { get; }

        IGraphNode TargetNode { get; set; }
    }
}
