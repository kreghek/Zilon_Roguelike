using System;
using System.Collections.Generic;
using Zilon.Logic.Logging;
using Zilon.Logic.Persons;
using Zilon.Logic.Tactics;
using Zilon.Logic.Tactics.Events;
using Zilon.Logic.Tactics.Initialization;
using Zilon.Logic.Tactics.Map;

namespace Zilon.Logic.Services
{
    public sealed class CombatService : ICombatService
    {
        private Random random = new Random();

        private readonly ICombatCommandResolver combatCommandResolver;

        public CombatService(ICombatCommandResolver combatCommandResolver) {
            this.combatCommandResolver = combatCommandResolver;
        }

        public Combat CreateCombat(CombatInitData initData)
        {
            var combat = new Combat
            {
                Map = initData.Map
            };

            CreateActors(combat, initData);

            return combat;
        }

        public CommandResult MoveCommand(Combat combat, ActorSquad actorSquad, MapNode targetNode)
        {
            return ExecuteCommand(
                context =>
                {
                    return combatCommandResolver.MoveSquad(combat, actorSquad, targetNode);
                });
        }

        private void CreateActors(Combat combat, CombatInitData initData)
        {
            var squads = new List<ActorSquad>();
            var teamIndex = 0;
            foreach (var playerData in initData.Players)
            {
                var teamLocation = combat.Map.TeamNodes[teamIndex];
                var squadLocations = CombatMap.GetSquadNodes(teamLocation, combat.Map.Nodes);
                for (var squadIndex = 0; squadIndex < playerData.Squads.Length; squadIndex++)
                {
                    var squadNode = squadLocations[random.Next(0, squadLocations.Length)];
                    var squad = playerData.Squads[squadIndex];

                    var actorSquad = new ActorSquad(squad, squadNode);
                    squads.Add(actorSquad);

                    for (var personIndex = 0; personIndex < squad.Persons.Length; personIndex++)
                    {
                        var person = squad.Persons[personIndex];

                        var actor = CreateActor(person);
                        actorSquad.Actors.Add(actor);
                    }
                }

                teamIndex++;
            }

            combat.Squads = squads.ToArray();
        }

        //TODO Создание актёров должен заниматься отдельный сервис
        private Actor CreateActor(Person person)
        {
            var actor = new Actor(person);
            return actor;
        }

        private CommandResult ExecuteCommand(Func<CommandContext, CommandResult> commandDelegate)
        {
            try
            {
                var context = GetCommandContext();
                if (!context.IsValid)
                {
                    Logger.TraceError(LogCodes.ErrorCommands, "Не валидный контекст комманды.\n" + string.Join("\n", context.Errors));

                    return new CommandResult
                    {
                        Type = CommandResultType.InvalidContext,
                        Errors = context.Errors
                    };
                }

                var result = commandDelegate(context);

                return result;
            }
            catch (Exception exception)
            {
                Logger.TraceError(LogCodes.ErrorCommands, "Ошибка при выполнении команды", exception);

                return new CommandResult
                {
                    Type = CommandResultType.InnerError,
                    Errors = new[] { exception.ToString() }
                };
            }
        }

        private CommandContext GetCommandContext()
        {
            var context = new CommandContext
            {
                
            };

            return context;
        }
    }
}
