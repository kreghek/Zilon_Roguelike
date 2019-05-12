namespace Zilon.Bot.Players
{
    public interface ILogicStateSelector
    {
        ILogicStateSelectorResult CheckConditions();
        ILogicState GenerateLogic(ILogicStateSelectorResult result);
    }
}
