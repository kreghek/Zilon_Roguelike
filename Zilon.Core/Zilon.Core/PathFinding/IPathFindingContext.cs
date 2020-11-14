using Zilon.Core.Graphs;

namespace Zilon.Core.PathFinding
{
    public interface IPathFindingContext
    {
        IEnumerable<IGraphNode> GetNext(IGraphNode current);
    }
}