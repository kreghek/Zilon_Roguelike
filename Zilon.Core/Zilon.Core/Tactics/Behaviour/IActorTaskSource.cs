using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    public interface IActorTaskSource
    {
        IActorTask[] GetActorTasks(IMap map, IActor[] actors);
    }
}