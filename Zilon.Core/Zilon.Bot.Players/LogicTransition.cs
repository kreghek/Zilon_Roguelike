using System;

namespace Zilon.Bot.Players
{
    public class LogicTransition
    {
        public LogicTransition(ILogicStateTrigger trigger, ILogicState nextState)
        {
            Trigger = trigger ?? throw new ArgumentNullException(nameof(trigger));
            NextState = nextState ?? throw new ArgumentNullException(nameof(nextState));
        }

        public ILogicStateTrigger Trigger { get; }
        public ILogicState NextState { get; }

        public override string ToString()
        {
            return $"{Trigger}: {NextState}";
        }
    }
}
