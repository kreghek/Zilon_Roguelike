using Zilon.Core.Tactics;

namespace Zilon.Bot.Players
{
    public interface ILogicStateTrigger
    {
        bool Test(IActor actor, ILogicState currentState, ILogicStateData data);

        ILogicStateData CreateData(IActor actor);
    }
}
