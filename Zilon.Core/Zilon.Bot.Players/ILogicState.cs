using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players
{
    public interface ILogicState
    {
        ILogicStateData CreateData(IActor actor);

        IActorTask GetTask(IActor actor, ILogicStateData data);
    }
}
