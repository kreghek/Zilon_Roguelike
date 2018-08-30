namespace Zilon.Core.Tactics.Spatial.PathFinding
{
    public interface IPathFindingContext
    {
        IActor Actor { get; }

        IMapNode TargetNode { get; set; }
    }
}
