using System;
using Zilon.Bot.Players.Strategies;
using Zilon.Bot.Sdk;
using Zilon.Core.Players;
using Zilon.Core.Tactics;

namespace Zilon.Bot.Players
{
    public sealed class HumanBotActorTaskSource : BotActorTaskSourceBase
    {
        private IBotSettings _botSettings;

        public HumanBotActorTaskSource(HumanPlayer player) : base(player)
        {
            
        }

        protected override ILogicStrategy GetLogicStrategy(IActor actor)
        {
            if (_botSettings == null)
            {
                return new LogicTreeStrategy(actor, LogicStateTreePatterns.DefaultHumanBot);
            }

            var normalizedMode = _botSettings.Mode?.Trim().ToUpperInvariant();
            switch (normalizedMode)
            {
                case "":
                case null:
                    return new LogicTreeStrategy(actor, LogicStateTreePatterns.DefaultHumanBot);

                case "JOE":
                    return new LogicTreeStrategy(actor, LogicStateTreePatterns.JoeHumanBot);

                case "DUNCAN":
                    return new LogicTreeStrategy(actor, LogicStateTreePatterns.DuncanHumanBot);

                case "MONSTER":
                    return new LogicTreeStrategy(actor, LogicStateTreePatterns.Monster);

                default:
                    throw new NotSupportedException();
            }
        }

        public override void Configure(IBotSettings botSettings)
        {
            _botSettings = botSettings;
        }
    }
}
