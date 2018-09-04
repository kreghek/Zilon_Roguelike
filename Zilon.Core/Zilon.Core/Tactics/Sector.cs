using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{

    public class Sector : ISector
    {
        private readonly List<IActorTask> _tasks;

        private readonly TaskIniComparer _taskIniComparer;

        private readonly IActorManager _actorManager;

        private readonly IPropContainerManager _propContainerManager;

        public event EventHandler ActorExit;

        public IMapNode[] ExitNodes { get; set; }

        public IMap Map { get; }

        public Dictionary<IActor, IPatrolRoute> PatrolRoutes { get; }

        public IMapNode[] StartNodes { get; set; }

        public Sector(IMap map,
            IActorManager actorManager,
            IPropContainerManager propContainerManager)
        {
            _tasks = new List<IActorTask>();
            _taskIniComparer = new TaskIniComparer();
            PatrolRoutes = new Dictionary<IActor, IPatrolRoute>();

            Map = map ?? throw new ArgumentException("Не передана карта сектора.", nameof(map));

            _actorManager = actorManager;
            _propContainerManager = propContainerManager;

            _actorManager.Added += ActorManager_Added;
            _propContainerManager.Added += PropContainerManager_Added;
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
            //// Определяем команды на текущий ход
            //CollectActorTasks();

            //// Выполняем все команды на текущий ход
            //ExecuteActorTasks();

            UpdateSurvivals();

            UpdateActorEffects();

            // Определяем, не покинули ли актёры игрока сектор.
            DetectSectorExit();
        }

        private void UpdateActorEffects()
        {
            foreach (var actor in _actorManager.Actors)
            {
                var effects = actor.Person.Effects;
                foreach (var effect in effects.Items)
                {
                    if (effect is IActorStateEffect actorEffect)
                    {
                        actorEffect.Apply(actor.State);
                    }
                }
            }
        }

        private void UpdateSurvivals()
        {
            var actors = _actorManager.Actors.ToArray();
            foreach (var actor in actors)
            {
                var survival = actor.Person.Survival;
                if (survival == null)
                {
                    continue;
                }

                survival.Update();
            }
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

                if (task.Actor.State.IsDead)
                {
                    // Произошло, если сначала актёру выдали задачу, но он ниже по инициативе и помер.
                    continue;
                }

                task.Execute();
            }
        }

        /// <summary>
        /// Определяем задачи актёров на текущий ход по всем зарегистрированным источникам задач.
        /// Результат записывается во внутренний список _tasks.
        /// </summary>
        //private void CollectActorTasks()
        //{
        //    _tasks.Clear();
        //    foreach (var behaviourSource in BehaviourSources)
        //    {
        //        var currentSourceTasks = behaviourSource.GetActorTasks(Map, _actorManager);
        //        if (currentSourceTasks != null)
        //        {
        //            _tasks.AddRange(currentSourceTasks);
        //        }
        //    }
        //}

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


        private void PropContainerManager_Added(object sender, ManagerItemsChangedArgs<IPropContainer> e)
        {
            foreach (var container in e.Items)
            {
                Map.HoldNode(container.Node, container);
            }
        }

        private void ActorManager_Added(object sender, ManagerItemsChangedArgs<IActor> e)
        {
            foreach (var actor in e.Items)
            {
                Map.HoldNode(actor.Node, actor);

                actor.State.Dead += ActorState_Dead;
            }
        }

        private void ActorState_Dead(object sender, EventArgs e)
        {
            var actor = _actorManager.Actors.Single(x => x.State == sender);
            Map.ReleaseNode(actor.Node, actor);
            _actorManager.Remove(actor);
            actor.State.Dead -= ActorState_Dead;
        }

        private void DoActorExit()
        {
            var e = new EventArgs();
            ActorExit?.Invoke(this, e);
        }
    }
}
