using System;

namespace Zilon.Bot.Players
{
    public class LogicTransition
    {
        public LogicTransition(ILogicStateTrigger selector, ILogicState nextState)
        {
            Selector = selector ?? throw new ArgumentNullException(nameof(selector));
            NextState = nextState ?? throw new ArgumentNullException(nameof(nextState));
        }

        public ILogicStateTrigger Selector { get; }
        public ILogicState NextState { get; }
    }
}
