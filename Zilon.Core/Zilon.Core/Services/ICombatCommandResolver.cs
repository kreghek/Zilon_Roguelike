namespace Zilon.Core.Services
{
    using Zilon.Core.Tactics;
    using Zilon.Core.Tactics.Events;
    using Zilon.Core.Tactics.Spatial;

    public interface ICombatCommandResolver
    {
        CommandResult MoveSquad(Sector combat, Actor actorSquad, HexNode targetNode);
    }
}