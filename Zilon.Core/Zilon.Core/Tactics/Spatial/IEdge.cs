namespace Zilon.Core.Tactics.Spatial
{
    /// <summary>
    /// Интерфейс ребра графа карты.
    /// </summary>
    public interface IEdge
    {
        /// <summary>
        /// Соединённые узлы карты.
        /// </summary>
        IMapNode[] Nodes { get; }
    }
}