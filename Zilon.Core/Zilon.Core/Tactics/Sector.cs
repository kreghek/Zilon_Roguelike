using System;
using System.Collections.Generic;
using System.Linq;
using Zilon.Core.Players;
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

        private readonly IPropContainerManager _propContainerManager;

        public event EventHandler ActorExit;

        public IActorTaskSource[] BehaviourSources { get; set; }

        public IMapNode[] ExitNodes { get; set; }

        public Sector(IMap map, 
            IActorManager actorManager, 
            IPropContainerManager propContainerManager)
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
            _propContainerManager = propContainerManager;
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
            CollectActorTasks();

            // Выполняем все команды на текущий ход
            ExecuteActorTasks();

            // Определяем, не покинули ли актёры игрока сектор.
            DetectSectorExit();
        }

        /// <summary>
        /// Выполнение задач на текущий ход. Задачи берутся из внутреннего списка _tasks.
        /// </summary>
        private void ExecuteActorTasks()
        {
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

        /// <summary>
        /// Определяем задачи актёров на текущий ход по всем зарегистрированным источникам задач.
        /// Результат записывается во внутренний список _tasks.
        /// </summary>
        private void CollectActorTasks()
        {
            _tasks.Clear();
            foreach (var behaviourSource in BehaviourSources)
            {
                var currentSourceTasks = behaviourSource.GetActorTasks(_map, _actorManager);
                if (currentSourceTasks != null)
                {
                    _tasks.AddRange(currentSourceTasks);
                }
            }
        }

        /// <summary>
        /// Определяет, находятся ли актёры игрока в точках выхода их сектора.
        /// </summary>
        private void DetectSectorExit()
        {
            var allExit = true;

            foreach (var actor in _actorManager.Actors)
            {
                if (actor.Owner is HumanPlayer && !ExitNodes.Contains(actor.Node))
                {
                    allExit = false;
                }
            }

            if (allExit)
            {
                DoActorExit();
            }
        }

        protected void DoActorExit()
        {
            var e = new EventArgs();
            ActorExit?.Invoke(this, e);
        }
    }
}
