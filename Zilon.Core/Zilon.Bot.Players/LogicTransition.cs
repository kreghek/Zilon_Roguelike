using System;
using System.Diagnostics.CodeAnalysis;

namespace Zilon.Bot.Players
{
    public class LogicTransition
    {
        public LogicTransition(ILogicStateTrigger trigger, ILogicState nextState)
        {
            Trigger = trigger ?? throw new ArgumentNullException(nameof(trigger));
            NextState = nextState ?? throw new ArgumentNullException(nameof(nextState));
        }

        public ILogicState NextState { get; }

        public ILogicStateTrigger Trigger { get; }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return $"{Trigger}: {NextState}";
        }
    }
}