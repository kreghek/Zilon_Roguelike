namespace Zilon.Core.Graphs
{
    /// <summary>
    /// Граф с узлами, которые имеют координаты.
    /// </summary>
    /// <remarks>
    /// Этот граф учитывает, что узлы могут иметь координаты, а рёбра - проходимость.
    /// </remarks>
    public interface ISpatialGraph: IGraph
    {
        /// <summary>
        /// Рассчитывает рассточние между двумя узлами графа.
        /// </summary>
        /// <param name="currentNode">Стартовый узел.</param>
        /// <param name="targetNode">Целевой узел.</param>
        /// <returns> Целочисленное значение расстояния. </returns>
        int DistanceBetween(IGraphNode currentNode, IGraphNode targetNode);
    }
}
