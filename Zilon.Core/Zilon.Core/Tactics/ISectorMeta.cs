using Zilon.Core.Tactics.Behaviour.Bots;

namespace Zilon.Core.Tactics
{
    public interface ISectorMeta
    {
        IPatrolRoute[] AvailablePatrolRoutes { get; }
    }
}
