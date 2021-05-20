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

        public ILogicStateTrigger FiredTrigger { get; }

        public ILogicState Logic { get; }

        public override string ToString()
        {
            return $"{FiredTrigger} -> {Logic}";
        }
    }
}