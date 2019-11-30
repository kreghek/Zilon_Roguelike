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
        /// Стоимость перемещение.
        /// </summary>
        public int MovementCost { get; set; }

        /// <summary>
        /// Суммарная стоимость.
        /// </summary>
        public int TotalCost => 0;
    }
}
