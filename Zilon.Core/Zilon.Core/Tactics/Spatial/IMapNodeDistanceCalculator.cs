using Zilon.Core.Graphs;

namespace Zilon.Core.Tactics.Spatial
{
    public interface IMapNodeDistanceCalculator<in TNodeMap> where TNodeMap : IGraphNode
    {
        int GetDistance(TNodeMap currentNode, TNodeMap targetNode);
    }
}