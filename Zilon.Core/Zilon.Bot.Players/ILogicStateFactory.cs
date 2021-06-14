namespace Zilon.Bot.Players
{
    public interface ILogicStateFactory
    {
        ILogicState CreateLogic<T>() where T : ILogicState;

        ILogicStateTrigger CreateTrigger<T>() where T : ILogicStateTrigger;
    }
}