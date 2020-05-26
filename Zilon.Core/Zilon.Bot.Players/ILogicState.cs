using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players
{
    public interface ILogicState
    {
        IActorTask GetTask(IActor actor, ISectorTaskSourceContext context, ILogicStrategyData strategyData);

        bool Complete { get; }

        void Reset();
    }
}
