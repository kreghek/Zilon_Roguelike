using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players
{
    public interface ILogicState
    {
        bool Complete { get; }

        IActorTask GetTask(IActor actor, ISectorTaskSourceContext context, ILogicStrategyData strategyData);

        void Reset();
    }
}