using System;

using Zilon.Bot.Players.Strategies;
using Zilon.Bot.Sdk;
using Zilon.Core.Persons;
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
            switch (actor.Person)
            {
                case MonsterPerson _:
                    return new LogicTreeStrategy(actor, LogicStateTreePatterns.Monster);

                case CitizenPerson _:
                    return new LogicTreeStrategy(actor, LogicStateTreePatterns.Citizen);

                default:
                    throw new NotSupportedException($"{actor.Person.GetType()} не поддерживается.");
            }
            
        }
    }
}
