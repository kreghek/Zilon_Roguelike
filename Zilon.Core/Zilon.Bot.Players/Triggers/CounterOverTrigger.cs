using Zilon.Core.Tactics;

namespace Zilon.Bot.Players.Triggers
{
    public sealed class CounterOverTrigger : ILogicStateTrigger
    {
        public ILogicStateData CreateData(IActor actor)
        {
            return new CounterTriggerData(3);
        }

        public bool Test(IActor actor, ILogicState currentState, ILogicStateData data)
        {
            var triggerData = (CounterTriggerData)data;

            triggerData.CounterDown();

            return triggerData.CounterIsOver;
        }
    }
}
