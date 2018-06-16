using System;
using System.Collections.Generic;
using Zilon.Core.Persons;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Map;

namespace Zilon.Core.Tactics
{

    public class Sector
    {
        private List<ICommand> _commands;
        
        public CombatMap Map { get; }

        public IBehaviourSource[] BehaviourSources { get; set; }
        
        public List<IActor> Actors { get; }

        public Sector(CombatMap map)
        {
            if (map == null)
            {
                throw new ArgumentException("Не передана карта сектора.", nameof(map));
            }
            
            _commands = new List<ICommand>();

            Map = map;
            Actors = new List<IActor>();
        }

        public IActor AddActor(IPerson person, MapNode node)
        {
            var actor = new Actor(person, node);
            Actors.Add(actor);
            return actor;
        }

        public void Update()
        {
            // Определяем команды на текущий ход
            _commands.Clear();
            foreach (var behaviourSource in BehaviourSources)
            {
                var commands = behaviourSource.GetCommands(Map, Actors.ToArray());
                if (commands != null)
                {
                    _commands.AddRange(commands);
                }
            }

            
            // Выполняем все команды на текущий ход
            //TODO Сделать сортировку команд по инициативе актёра.
            foreach (var command in _commands)
            {
                if (!command.IsComplete)
                {
                    command.Execute();
                }
            }
        }
    }
}
