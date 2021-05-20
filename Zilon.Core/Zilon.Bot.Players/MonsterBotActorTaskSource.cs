﻿using System;

using Zilon.Bot.Players.Strategies;
using Zilon.Bot.Sdk;
using Zilon.Core.Persons;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players
{
    public sealed class MonsterBotActorTaskSource<TContext> : BotActorTaskSourceBase<TContext>
        where TContext : class, ISectorTaskSourceContext
    {
        private readonly LogicStateTreePatterns _logicStateTreePatterns;

        public MonsterBotActorTaskSource(LogicStateTreePatterns logicStateTreePatterns)
        {
            _logicStateTreePatterns = logicStateTreePatterns;
        }

        public override void Configure(IBotSettings botSettings)
        {
            // Монстров не нужно конфигурировать.
        }

        protected override ILogicStrategy GetLogicStrategy(IActor actor)
        {
            if (actor is null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            switch (actor.Person)
            {
                case MonsterPerson _:
                    return new LogicTreeStrategy(actor, _logicStateTreePatterns.Monster);

                case HumanPerson _:

                default:
                    throw new NotSupportedException($"{actor.Person.GetType()} не поддерживается.");
            }
        }
    }
}