using System.Collections.Generic;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour.Bots
{
    /// <summary>
    /// Источник решений для AI.
    /// </summary>
    public interface IDecisionSource
    {
        /// <summary>
        /// Выбирает длительность ожидания в ходах.
        /// </summary>
        /// <param name="min"> Минимальное количество ходов ожидания. </param>
        /// <param name="max"> Максимальное количество ходов ожидания. </param>
        /// <returns> Количество ходов ожидания. </returns>
        int SelectIdleDuration(int min, int max);

        IMapNode SelectTargetRoamingNode(IEnumerable<IMapNode> mapNodes);
    }
}
