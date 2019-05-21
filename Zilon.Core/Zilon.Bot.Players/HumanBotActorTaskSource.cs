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
                return new LogicTreeStrategy(actor, LogicStateTreePatterns.DefaultHumanBot)
                {
                    WriteStateChanges = true
                };
            }

            var normalizedMode = _botSettings.Mode?.Trim().ToUpperInvariant();
            switch (normalizedMode)
            {
                case "":
                case null:
                    return new LogicTreeStrategy(actor, LogicStateTreePatterns.DefaultHumanBot)
                    {
                        WriteStateChanges = true
                    };

                case "JOE":
                    return new LogicTreeStrategy(actor, LogicStateTreePatterns.JoeHumanBot)
                    {
                        WriteStateChanges = true
                    };

                case "DUNCAN":
                    return new LogicTreeStrategy(actor, LogicStateTreePatterns.DuncanHumanBot)
                    {
                        WriteStateChanges = true
                    };

                case "MONSTER":
                    return new LogicTreeStrategy(actor, LogicStateTreePatterns.Monster)
                    {
                        WriteStateChanges = true
                    };

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
