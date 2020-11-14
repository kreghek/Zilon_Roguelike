namespace Zilon.Core.Tactics.Spatial
{
    public sealed class HexMapNodeDistanceCalculator : IMapNodeDistanceCalculator<HexNode>
    {
        public int GetDistance(HexNode currentNode, HexNode targetNode)
        {
            CubeCoords currentCoords = currentNode.CubeCoords;
            CubeCoords targetCoords = targetNode.CubeCoords;
            var distance = currentCoords.DistanceTo(targetCoords);

            return distance;
        }
    }
}