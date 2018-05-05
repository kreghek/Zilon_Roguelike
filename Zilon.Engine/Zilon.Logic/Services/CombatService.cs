using System;
using System.Collections.Generic;
using Zilon.Logic.Persons;
using Zilon.Logic.Tactics;
using Zilon.Logic.Tactics.Initialization;
using Zilon.Logic.Tactics.Map;

namespace Zilon.Logic.Services
{
    public class CombatService
    {
        private Random random = new Random();

        public Combat CreateCombat(CombatInitData initData)
        {
            var combat = new Combat
            {
                Map = initData.Map
            };

            CreateActors(combat, initData);

            return combat;
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
                    var squadData = playerData.Squads[squadIndex];

                    var squad = new ActorSquad
                    {
                        CurrentNode = squadNode
                    };

                    squads.Add(squad);

                    for (var personIndex = 0; personIndex < squadData.Persons.Length; personIndex++)
                    {
                        var person = squadData.Persons[personIndex];

                        var actor = CreateActor(person);
                        squad.Actors.Add(actor);
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
    }
}
