using System;

using Zilon.Bot.Players.Strategies;
using Zilon.Bot.Sdk;
using Zilon.Core.Persons;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players
{
    public sealed class HumanBotActorTaskSource<TContext> : BotActorTaskSourceBase<TContext>,
        IPluggableActorTaskSource<TContext> where TContext : class, ISectorTaskSourceContext
    {
        private readonly LogicStateTreePatterns _logicStateTreePatterns;
        private IBotSettings _botSettings;

        public HumanBotActorTaskSource(LogicStateTreePatterns logicStateTreePatterns)
        {
            _logicStateTreePatterns = logicStateTreePatterns;
        }

        public override void Configure(IBotSettings botSettings)
        {
            _botSettings = botSettings;
        }

        protected override ILogicStrategy GetLogicStrategy(IActor actor)
        {
            if (actor is null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            if (_botSettings == null)
            {
                if (actor.Person is HumanPerson)
                {
                    return new LogicTreeStrategy(actor, _logicStateTreePatterns.DefaultHumanBot)
                    {
                        WriteStateChanges = true
                    };
                }

                return new LogicTreeStrategy(actor, _logicStateTreePatterns.Monster) { WriteStateChanges = true };
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
                    return new LogicTreeStrategy(actor, _logicStateTreePatterns.Monster) { WriteStateChanges = true };

                default:
                    throw new NotSupportedException();
            }
        }
    }
}