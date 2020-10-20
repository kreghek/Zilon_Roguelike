using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Triggers
{
    public sealed class CounterOverTrigger : ILogicStateTrigger
    {
        private const int COUNTER_INITIAL_VALUE = 3;

        public CounterOverTrigger()
        {
            Counter = COUNTER_INITIAL_VALUE;
        }

        public int Counter { get; private set; }
        public bool CounterIsOver => Counter <= 0;

        public void Reset()
        {
            Counter = COUNTER_INITIAL_VALUE;
        }

        public bool Test(IActor actor, ISectorTaskSourceContext context, ILogicState currentState, ILogicStrategyData strategyData)
        {
            return CounterIsOver;
        }

        private void CounterDown()
        {
            Counter--;
        }

        public void Update()
        {
            CounterDown();
        }
    }
}