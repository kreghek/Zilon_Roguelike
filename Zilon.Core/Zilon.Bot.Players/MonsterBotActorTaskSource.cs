using Zilon.Bot.Players.Strategies;
using Zilon.Bot.Sdk;
using Zilon.Core.Players;
using Zilon.Core.Tactics;

namespace Zilon.Bot.Players
{
    public sealed class MonsterBotActorTaskSource : BotActorTaskSourceBase
    {
        public MonsterBotActorTaskSource(IBotPlayer player) : base(player)
        {
        }

        public override void Configure(IBotSettings botSettings)
        {
            // Монстров не нужно конфигурировать.
        }

        protected override ILogicStrategy GetLogicStrategy(IActor actor)
        {
            return new LogicTreeStrategy(actor, LogicStateTreePatterns.Monster);
        }
    }
}
