using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Player
{
    public interface ILogicState
    {
        IActorTask GetCurrentTask();
    }
}
