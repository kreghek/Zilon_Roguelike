using Zilon.Core.Graphs;

namespace Zilon.Core.Tactics.Behaviour.Bots
{
    /// <summary>
    ///     Интерфейс маршрута патруллирования.
    /// </summary>
    public interface IPatrolRoute
    {
        /// <summary>
        ///     Узлы карты для патруллирования.
        /// </summary>
        /// <remarks>
        ///     Находятся в том порядке, в котором их нужно обходить.
        /// </remarks>
        IGraphNode[] Points { get; }
    }
}