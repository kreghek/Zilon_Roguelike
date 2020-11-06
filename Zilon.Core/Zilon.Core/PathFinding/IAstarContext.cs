using Zilon.Core.Graphs;

namespace Zilon.Core.PathFinding
{
    public interface IAstarContext : IPathFindingContext
    {
        int GetDistanceBetween(IGraphNode current, IGraphNode target);
    }
}