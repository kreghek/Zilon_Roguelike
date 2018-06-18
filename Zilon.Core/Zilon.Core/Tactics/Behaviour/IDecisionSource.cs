using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Источник решений для AI.
    /// </summary>
    public interface IDecisionSource
    {
        /// <summary>
        /// Выбирает точку маршрута.
        /// </summary>
        /// <param name="map"> Карта сектора. </param>
        /// <returns> Возвращает узел указанной карты, который является точкой патрулирования. </returns>
        IMapNode SelectPatrolPoint(IMap map);
    }
}
