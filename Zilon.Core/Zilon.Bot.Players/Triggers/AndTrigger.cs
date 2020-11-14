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
            foreach (ILogicStateTrigger trigger in _triggers)
            {
                trigger.Reset();
            }
        }

        public bool Test(IActor actor, ISectorTaskSourceContext context, ILogicState currentState,
            ILogicStrategyData strategyData)
        {
            foreach (ILogicStateTrigger trigger in _triggers)
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
            foreach (ILogicStateTrigger trigger in _triggers)
            {
                trigger.Update();
            }
        }
    }
}