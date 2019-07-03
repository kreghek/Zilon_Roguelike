namespace Zilon.Core.Tactics.Spatial
{
    public interface IMapNodeDistanceCalculator<TNodeMap> where TNodeMap: IMapNode
    {
        int GetDistance(TNodeMap currentNode, TNodeMap targetNode);
    }
}
