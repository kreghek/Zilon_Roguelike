using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    public interface IActorTaskSource
    {
        IActorTask[] GetActorTasks(IHexMap map, IActor[] actors);
    }
}