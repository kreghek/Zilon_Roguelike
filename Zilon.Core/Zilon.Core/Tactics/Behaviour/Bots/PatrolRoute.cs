using Zilon.Core.Graphs;

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
        public IGraphNode[] Points { get; }

        public PatrolRoute(params IGraphNode[] points)
        {
            Points = points;
        }
    }
}