using System;

using Zilon.Bot.Players.Strategies;
using Zilon.Bot.Sdk;
using Zilon.Core.Players;
using Zilon.Core.Tactics;

namespace Zilon.Bot.Players
{
    public sealed class HumanBotActorTaskSource : BotActorTaskSourceBase, IPluggableActorTaskSource
    {
        private IBotSettings _botSettings;
        private readonly LogicStateTreePatterns _logicStateTreePatterns;

        public HumanBotActorTaskSource(HumanPlayer player, LogicStateTreePatterns logicStateTreePatterns) : base(player)
        {
            _logicStateTreePatterns = logicStateTreePatterns;
        }

        protected override ILogicStrategy GetLogicStrategy(IActor actor)
        {
            if (actor is null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            if (_botSettings == null)
            {
                return new LogicTreeStrategy(actor, _logicStateTreePatterns.DefaultHumanBot)
                {
                    WriteStateChanges = true
                };
            }

            var normalizedMode = _botSettings.Mode?.Trim().ToUpperInvariant();
            switch (normalizedMode)
            {
                case "":
                case null:
                    return new LogicTreeStrategy(actor, _logicStateTreePatterns.DefaultHumanBot)
                    {
                        WriteStateChanges = true
                    };

                case "JOE":
                    return new LogicTreeStrategy(actor, _logicStateTreePatterns.JoeHumanBot)
                    {
                        WriteStateChanges = true
                    };

                case "DUNCAN":
                    return new LogicTreeStrategy(actor, _logicStateTreePatterns.DuncanHumanBot)
                    {
                        WriteStateChanges = true
                    };

                case "MONSTER":
                    return new LogicTreeStrategy(actor, _logicStateTreePatterns.Monster)
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