using Zilon.Core.Tactics;

namespace Zilon.Bot.Players
{
    public interface ILogicStateTrigger
    {
        bool Test(IActor actor, ILogicStateData data);

        ILogicStateData CreateData(IActor actor);
    }
}
