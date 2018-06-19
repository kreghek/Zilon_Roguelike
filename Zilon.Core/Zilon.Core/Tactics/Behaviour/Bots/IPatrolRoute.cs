using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour.Bots
{
    public interface IPatrolRoute
    {
        IMapNode[] Points { get; }
    }
}