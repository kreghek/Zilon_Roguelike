using Zilon.Core.Graphs;

namespace Zilon.Core.PathFinding
{
    /// <summary>
    /// Внутреняя структура для алгоритма поиска пути A*.
    /// </summary>
    internal class AStarData
    {
        /// <summary>
        /// Родительский узел.
        /// </summary>
        public IGraphNode Parent { get; set; }

        /// <summary>
        /// Стоимость перемещение от стартовой вершины к этой вершине.
        /// Обычно обозначается, как g(x).
        /// </summary>
        public int MovementCost { get; set; }

        /// <summary>
        /// Оценка расстояния от текущей вершины до цели.
        /// </summary>
        public int EstimateCost { get; set; }

        /// <summary>
        /// Суммарная стоимость.
        /// </summary>
        public int TotalCost => MovementCost + EstimateCost;
    }
}
