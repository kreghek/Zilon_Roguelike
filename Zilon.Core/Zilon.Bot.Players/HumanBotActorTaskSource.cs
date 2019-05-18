using Zilon.Bot.Players.Strategies;
using Zilon.Core.Players;
using Zilon.Core.Tactics;

namespace Zilon.Bot.Players
{
    public sealed class HumanBotActorTaskSource : BotActorTaskSourceBase
    {
        public HumanBotActorTaskSource(HumanPlayer player) : base(player)
        {
        }

        protected override ILogicStrategy GetLogicStrategy(IActor actor)
        {
            return new LogicTreeStrategy(actor, LogicStateTreePatterns.HumanBot);
        }
    }
}
