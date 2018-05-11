namespace Zilon.Core.Services
{
    using Zilon.Core.Tactics;
    using Zilon.Core.Tactics.Events;
    using Zilon.Core.Tactics.Initialization;
    using Zilon.Core.Tactics.Map;

    public interface ICombatService
    {
        Combat CreateCombat(CombatInitData initData);
        //TODO Переделать на команды вместо отдельных методов.
        CommandResult MoveCommand(Combat combat, ActorSquad actorSquad, MapNode targetNode);
        CommandResult EndTurnCommand(Combat combat);
    }
}