namespace Zilon.Bot.Players
{
    public class LogicTransition
    {
        public ILogicStateSelector Selector { get; }
        public ILogicState NextState { get; }
    }
}
