using System.Collections.Generic;

using Zilon.Core.Graphs;

namespace Zilon.Core.PathFinding
{
    public interface IPathFindingContext
    {
        IEnumerable<IGraphNode> GetNext(IGraphNode current);

        int GetDistanceBetween(IGraphNode current, IGraphNode target);
    }
}
