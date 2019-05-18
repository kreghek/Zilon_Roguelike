using Zilon.Core.Persons;
using Zilon.Core.Tactics;

namespace Zilon.Bot.Players.Strategies
{
    public sealed class BotLogicStrategySelector : ILogicStrategySelector
    {
        public ILogicStrategy GetLogicStrategy(IActor actor)
        {
            if (actor.Person is HumanPerson)
            {
                return new LogicTreeStrategy(actor, LogicStateTreePatterns.HumanBot);
            }
            else
            {
                return new LogicTreeStrategy(actor, LogicStateTreePatterns.Monster);
            }
        }
    }
}
