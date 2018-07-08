using System;
using System.Collections.Generic;

using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{

    public class Sector : ISector
    {
        private readonly List<IActorTask> _tasks;

        private readonly TaskIniComparer _taskIniComparer;

        private readonly IMap _map;

        private readonly IActorManager _actorManager;

        public IActorTaskSource[] BehaviourSources { get; set; }

        public Sector(IMap map, IActorManager actorManager)
        {
#pragma warning disable IDE0016 // Use 'throw' expression
            if (map == null)
            {
                throw new ArgumentException("Не передана карта сектора.", nameof(map));
            }
#pragma warning restore IDE0016 // Use 'throw' expression

            _tasks = new List<IActorTask>();
            _taskIniComparer = new TaskIniComparer();

            _map = map;
            _actorManager = actorManager;
        }

        /// <summary>
        /// Обновление состояния сектора.
        /// </summary>
        /// <remarks>
        /// Выполняет ход игрового сектора.
        /// Собирает текущие задачи для всех актёров в секторе.
        /// Выполняет все задачи для каждого актёра.
        /// </remarks>
        public void Update()
        {
            // Определяем команды на текущий ход
            _tasks.Clear();
            foreach (var behaviourSource in BehaviourSources)
            {
                var commands = behaviourSource.GetActorTasks(_map, _actorManager);
                if (commands != null)
                {
                    _tasks.AddRange(commands);
                }
            }


            // Выполняем все команды на текущий ход
            _tasks.Sort(_taskIniComparer);
            //TODO Добавить тест, проверяющий, что задачи выполняются с учётом инициативы.
            foreach (var task in _tasks)
            {
                if (task.IsComplete)
                {
                    throw new InvalidOperationException("В выполняемых командах обнаружена заверщённая задача.");
                }

                if (task.Actor == null)
                {
                    throw new InvalidOperationException("В задаче потеряна связь с актёром.");
                }

                //TODO Добавить событие на смерть актёра в тасках
                // На клиенте выпадает ошибка при штатной ситуации - актёра убили до того,
                // как он выполнил задачу. Необходимо подписываться на событие и удалять/игнорировать задачу,
                // если актёр более не может выполнять задачи. А сюда она вообще не должна попадать.
                if (task.Actor.IsDead)
                {
                    throw new InvalidOperationException("Задача назначена мертвому актёру.");
                }

                task.Execute();
            }
        }
    }
}
