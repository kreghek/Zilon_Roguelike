namespace Zilon.Core.Services
{
    using Zilon.Core.Tactics;
    using Zilon.Core.Tactics.Events;
    using Zilon.Core.Tactics.Initialization;
    using Zilon.Core.Tactics.Map;

    public interface ICombatService
    {
        Combat CreateCombat(CombatInitData initData);
        CommandResult MoveCommand(Combat combat, ActorSquad actorSquad, MapNode targetNode);
    }
}