using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players
{
    public interface ILogicState
    {
        ILogicStateData CreateData(IActor actor);

        IActorTask GetCurrentTask(IActor actor, ILogicStateData data);

        bool Complete { get; }
    }
}
