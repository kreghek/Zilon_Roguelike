using System;

namespace Zilon.Bot.Players.Strategies
{
    public sealed class LogicStateTrack
    {
        public LogicStateTrack(ILogicState logic, ILogicStateTrigger firedTrigger)
        {
            Logic = logic ?? throw new ArgumentNullException(nameof(logic));
            FiredTrigger = firedTrigger;
        }

        public ILogicState Logic { get; }

        public ILogicStateTrigger FiredTrigger { get; }

        public override string ToString()
        {
            return $"{FiredTrigger} -> {Logic}";
        }
    }
}
