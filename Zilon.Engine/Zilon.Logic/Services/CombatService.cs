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
            var actors = new List<Actor>();
            var teamIndex = 0;
            foreach (var playerData in initData.Players)
            {
                var teamLocation = combat.Map.TeamNodes[teamIndex];
                var squadLocations = CombatMap.GetSquadNodes(teamLocation, combat.Map.Nodes);
                for (var squadIndex = 0; squadIndex < playerData.Squads.Length; squadIndex++)
                {
                    var squadLocation = squadLocations[random.Next(0, squadLocations.Length)];
                    var squad = playerData.Squads[squadIndex];

                    for (var personIndex = 0; personIndex < squad.Persons.Length; personIndex++)
                    {
                        var person = squad.Persons[personIndex];

                        var actor = CreateActor(person, squadLocation);
                        actors.Add(actor);
                    }
                }

                teamIndex++;
            }

            combat.Actors = actors.ToArray();
        }

        //TODO Создание актёров должен заниматься отдельный сервис
        private Actor CreateActor(Person person, MapNode squadlocation)
        {
            var actor = new Actor(person, squadlocation);
            return actor;
        }
    }
}
