namespace Zilon.Core.Graphs
{
    /// <summary>
    ///     Интерфейс ребра графа карты.
    /// </summary>
    public interface IGraphEdge
    {
        /// <summary>
        ///     Соединённые узлы карты.
        /// </summary>
        IGraphNode[] Nodes { get; }

        /// <summary>
        ///     Стоимость ребра.
        /// </summary>
        int Cost { get; }
    }
}