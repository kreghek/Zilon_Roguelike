namespace Zilon.Core.Tactics.Spatial.PathFinding
{
    public class AStarData
    {
        public IMapNode Parent;
        public int MovementCost;
        public int EstimateCost;
        public int TotalCost;
    }
}
