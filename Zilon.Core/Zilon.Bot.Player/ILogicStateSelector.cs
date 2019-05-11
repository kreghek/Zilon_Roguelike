namespace Zilon.Bot.Player
{
    public interface ILogicStateSelector
    {
        bool CheckConditions();
        ILogicState GenerateLogic();
    }
}
