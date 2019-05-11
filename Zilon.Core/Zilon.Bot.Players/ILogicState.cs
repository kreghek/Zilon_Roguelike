using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players
{
    public interface ILogicState
    {
        IActorTask GetCurrentTask();
    }
}
