using Zilon.Core.Graphs;
using Zilon.Core.Tactics;

namespace Zilon.Core.PathFinding
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
