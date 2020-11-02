using Zilon.Core.World;

namespace Zilon.Core.Tactics.Behaviour
{
    public interface IActorTaskContext
    {
        IGlobe Globe { get; }
        ISector Sector { get; }
    }
}