using Zilon.Logic.Tactics;
using Zilon.Logic.Tactics.Events;
using Zilon.Logic.Tactics.Initialization;
using Zilon.Logic.Tactics.Map;

namespace Zilon.Logic.Services
{
    public interface ICombatService
    {
        Combat CreateCombat(CombatInitData initData);
        CommandResult MoveCommand(Combat combat, ActorSquad actorSquad, MapNode targetNode);
    }
}