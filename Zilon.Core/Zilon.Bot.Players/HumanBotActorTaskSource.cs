using Zilon.Bot.Players.Strategies;
using Zilon.Bot.Sdk;
using Zilon.Core.Players;
using Zilon.Core.Tactics;

namespace Zilon.Bot.Players
{
    public sealed class HumanBotActorTaskSource : BotActorTaskSourceBase
    {
        private readonly IBotSettings _botSettings;

        public HumanBotActorTaskSource(HumanPlayer player, IBotSettings botSettings) : base(player)
        {
            _botSettings = botSettings;
        }

        protected override ILogicStrategy GetLogicStrategy(IActor actor)
        {
            return new LogicTreeStrategy(actor, LogicStateTreePatterns.HumanBot);
        }
    }
}
