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
        }

        public IActor Actor { get; }
    }
}
