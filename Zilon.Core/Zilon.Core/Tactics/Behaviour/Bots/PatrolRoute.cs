using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour.Bots
{

    /// <summary>
    /// Базовая реализация маршрута патруллирования.
    /// </summary>
    public class PatrolRoute : IPatrolRoute
    {
        /// <summary>
        /// Контрольные узлы патруллирования.
        /// </summary>
        /// <remarks>
        /// Если монстр находится на маршруте патруллирования, то он будет обходить эти узлы.
        /// </remarks>
        public IMapNode[] Points { get; }

        public PatrolRoute(params IMapNode[] points)
        {
            Points = points;
        }
    }
}
