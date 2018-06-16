using Zilon.Core.Tactics.Map;

namespace Zilon.Core.Tactics.Behaviour
{
    public interface IBehaviourSource
    {
        ICommand[] GetCommands(CombatMap map, IActor[] actors);
    }
}