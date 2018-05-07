using Zilon.Logic.Tactics;
using Zilon.Logic.Tactics.Events;
using Zilon.Logic.Tactics.Map;

namespace Zilon.Logic.Services
{
    public interface ICombatCommandResolver
    {
        CommandResult MoveSquad(Combat combat, ActorSquad actorSquad, MapNode targetNode);
    }
}