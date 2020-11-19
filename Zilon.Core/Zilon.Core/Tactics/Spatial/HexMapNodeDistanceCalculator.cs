namespace Zilon.Core.Tactics.Spatial
{
    public sealed class HexMapNodeDistanceCalculator : IMapNodeDistanceCalculator<HexNode>
    {
        public int GetDistance(HexNode currentNode, HexNode targetNode)
        {
            var currentCoords = currentNode.CubeCoords;
            var targetCoords = targetNode.CubeCoords;
            var distance = currentCoords.DistanceTo(targetCoords);

            return distance;
        }
    }
}