using Zilon.Core.Graphs;

namespace Zilon.Core.Tactics.Behaviour.Bots
{
    /// <summary>
    ///     Источник решений для AI.
    /// </summary>
    public interface IDecisionSource
    {
        /// <summary>
        ///     Выбирает длительность ожидания в ходах.
        /// </summary>
        /// <param name="min"> Минимальное количество ходов ожидания. </param>
        /// <param name="max"> Максимальное количество ходов ожидания. </param>
        /// <returns> Количество ходов ожидания. </returns>
        int SelectIdleDuration(int min, int max);

        /// <summary>
        ///     Выбор случайного узла для перемещения по карте сектора.
        /// </summary>
        /// <param name="mapNodes">Доступные для выбора узлы карты.</param>
        /// <returns>Возвращает узле карты сектора. Или null, если такого узла нет.</returns>
        [CanBeNull]
        IGraphNode SelectTargetRoamingNode([NotNull] [ItemNotNull] IEnumerable<IGraphNode> mapNodes);
    }
}