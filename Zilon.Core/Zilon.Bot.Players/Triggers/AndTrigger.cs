using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Triggers
{
    public sealed class AndTrigger : ICompositLogicStateTrigger
    {
        private readonly ILogicStateTrigger[] _triggers;

        public AndTrigger(params ILogicStateTrigger[] triggers)
        {
            _triggers = triggers;
        }

        public void Reset()
        {
            foreach (var trigger in _triggers)
            {
                trigger.Reset();
            }
        }

        public bool Test(IActor actor, ISectorTaskSourceContext context, ILogicState currentState, ILogicStrategyData strategyData)
        {
            foreach (var trigger in _triggers)
            {
                if (!trigger.Test(actor, context, currentState, strategyData))
                {
                    return false;
                }
            }

            return true;
        }

        public void Update()
        {
            foreach (var trigger in _triggers)
            {
                trigger.Update();
            }
        }
    }
}