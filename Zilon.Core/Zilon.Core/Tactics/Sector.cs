using System;
using System.Collections.Generic;
using Zilon.Core.Persons;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{

    public class Sector
    {
        private readonly List<IActorTask> _commands;
        
        public IMap Map { get; }

        public IActorTaskSource[] BehaviourSources { get; set; }
        
        public List<IActor> Actors { get; }

        public Sector(IMap map)
        {
            if (map == null)
            {
#pragma warning disable IDE0016 // Use 'throw' expression
                throw new ArgumentException("Не передана карта сектора.", nameof(map));
#pragma warning restore IDE0016 // Use 'throw' expression
            }
            
            _commands = new List<IActorTask>();

            Map = map;
            Actors = new List<IActor>();
        }

        public IActor AddActor(IPerson person, HexNode node)
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
                var commands = behaviourSource.GetActorTasks(Map, Actors.ToArray());
                if (commands != null)
                {
                    _commands.AddRange(commands);
                }
            }


            // Выполняем все команды на текущий ход
            //TODO Сделать сортировку команд по инициативе актёра.
            foreach (var command in _commands)
            {
                if (command.IsComplete)
                {
                    throw new InvalidOperationException("В выполняемых командах обнаружена заверщённая задача.");
                }

                if (command.Actor.IsDead)
                {
                    throw new InvalidOperationException("Задача назначена мертвому актёру.");
                }

                command.Execute();
            }
        }
    }
}
