using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    public interface IBehaviourSource
    {
        ICommand[] GetCommands(IMap map, IActor[] actors);
    }
}