using Zilon.Core.Graphs;

namespace Zilon.Core.Tactics.Spatial.PathFinding
{
    /// <summary>
    /// Базовая реализация контекста поиска пути.
    /// </summary>
    public struct PathFindingContext : IPathFindingContext
    {
        public PathFindingContext(IActor actor)
        {
            Actor = actor;
            TargetNode = null;
        }

        public IActor Actor { get; }

        public IGraphNode TargetNode { get; set; }
    }
}
