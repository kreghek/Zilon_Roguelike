using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    public interface IActorTaskSource
    {
        IActorTask[] GetIntents(IMap map, IActor[] actors);
    }
}