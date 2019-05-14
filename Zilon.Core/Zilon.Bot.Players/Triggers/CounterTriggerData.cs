namespace Zilon.Bot.Players.Triggers
{
    public sealed class CounterTriggerData: ILogicStateData
    {
        public CounterTriggerData(int initialCounter)
        {
            Counter = initialCounter;
        }

        public int Counter { get; private set; }

        public void CounterDown()
        {
            Counter--;
        }

        public bool CounterIsOver => Counter <= 0;
    }
}
