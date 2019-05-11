namespace Zilon.Bot.Players
{
    public interface ILogicStateSelector
    {
        bool CheckConditions();
        ILogicState GenerateLogic();
    }
}
