namespace Zilon.Bot.Players
{
    public interface ILogicStateFactory
    {
        ILogicState CreateLogic<T>();

        ILogicStateTrigger CreateTrigger<T>();
    }
}
