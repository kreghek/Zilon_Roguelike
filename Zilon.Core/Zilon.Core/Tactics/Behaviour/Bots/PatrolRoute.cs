using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour.Bots
{

    /// <summary>
    /// Базовая реализация маршрута патруллирования.
    /// </summary>
    public class PatrolRoute : IPatrolRoute
    {
        public IMapNode[] Points { get; }

        public PatrolRoute(IMapNode[] points)
        {
            Points = points;
        }
    }
}
