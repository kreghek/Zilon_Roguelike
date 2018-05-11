namespace Zilon.Core.Services
{
    using Zilon.Core.Tactics;
    using Zilon.Core.Tactics.Events;
    using Zilon.Core.Tactics.Map;

    public interface ICombatCommandResolver
    {
        CommandResult MoveSquad(Combat combat, ActorSquad actorSquad, MapNode targetNode);
    }
}