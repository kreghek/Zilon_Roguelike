namespace Zilon.Core.Graphs
{
    /// <summary>
    /// Интерфейс ребра графа карты.
    /// </summary>
    public interface IGraphEdge
    {
        /// <summary>
        /// Стоимость ребра.
        /// </summary>
        int Cost { get; }

        /// <summary>
        /// Соединённые узлы карты.
        /// </summary>
        IGraphNode[] Nodes { get; }
    }
}