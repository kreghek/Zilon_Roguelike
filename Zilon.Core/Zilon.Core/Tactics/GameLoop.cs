using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Tactics
{
    public sealed class GameLoop : IGameLoop
    {
        private int _turnCounter;

        private Dictionary<IActor, TaskState> _taskDict;

        private readonly ISectorManager _sectorManager;

        public event EventHandler Updated;

        public GameLoop(ISectorManager sectorManager)
        {
            _sectorManager = sectorManager;

            _taskDict = new Dictionary<IActor, TaskState>();
        }

        public IActorTaskSource[] ActorTaskSources { get; set; }

        public void Update()
        {
            if (ActorTaskSources is null)
            {
                throw new InvalidOperationException("Не заданы источники команд");
            }

            // Алгоритм:
            // Нужно определить всех актёров, которые могут выполнить действия до
            // основного персонажа, управляемого игроком.
            // Они выполняют свои действия.
            // После этого выполняет действие основой актёр.
            // Если общий счётчик хода достигаем лимита, то выполняется начало следующего
            // игрового цикла - обновление голода/жажды и т.д.

            // В общем случае выглядит, как очередь задач от всех актёров.
            // Как только до актёра доходит очередь (признак - он выполнил текущую задачу),
            // он генерирует следующую задачу и она ставится в очередь.

            var actorsWithoutTasks = GetActorsWithoutTasks(_sectorManager.CurrentSector.ActorManager, _taskDict);
            GenerateActorTasksAndPutInDict(actorsWithoutTasks, _taskDict);
            ProcessTasks(_taskDict);
            _turnCounter++;
            if (_turnCounter >= 1000)
            {
                _turnCounter = 1000 - _turnCounter;

                _sectorManager.CurrentSector.Update();

                Updated?.Invoke(this, new EventArgs());
            }
        }

        private void ProcessTasks(Dictionary<IActor, TaskState> taskDict)
        {
            var states = taskDict.Values;
            foreach (var state in states)
            {
                state.UpdateCounter();
                if (state.TaskIsExecuting)
                {
                    state.Task.Execute();
                }
            }

            // удаляем выполненные задачи.
            foreach (var taskStatePair in taskDict.ToArray())
            {
                if (taskStatePair.Value.TaskComplete)
                {
                    taskDict.Remove(taskStatePair.Key);
                }
            }
        }

        private class TaskState
        {
            private readonly int _valueToExecute;

            public TaskState(IActorTask task)
            {
                Task = task ?? throw new ArgumentNullException(nameof(task));
                Counter = Task.Cost;
                _valueToExecute = Task.Cost / 2;
            }

            public IActorTask Task { get; }
            public int Counter { get; private set; }
            public void UpdateCounter()
            {
                Counter--;
            }

            public bool TaskIsExecuting { get => Counter <= _valueToExecute; }

            public bool TaskComplete { get => Counter <= 0; }
        }

        private void GenerateActorTasksAndPutInDict(IEnumerable<IActor> actors, Dictionary<IActor, TaskState> taskDict)
        {
            foreach (var actor in actors)
            {
                foreach (var taskSource in ActorTaskSources)
                {
                    var actorTasks = taskSource.GetActorTasks(actor);

                    if (actorTasks != null)
                    {
                        var task = actorTasks.First();

                        var state = new TaskState(task);
                        taskDict.Add(actor, state);
                    }
                }
            }
        }

        private IEnumerable<IActor> GetActorsWithoutTasks(IActorManager actorManager, Dictionary<IActor, TaskState> taskDict)
        {
            foreach (var actor in actorManager.Items)
            {
                if (!taskDict.TryGetValue(actor, out _))
                {
                    yield return actor;                         
                }
            }
        }

        private void ProcessActor(IActor actor)
        {
            foreach (var taskSource in ActorTaskSources)
            {
                var actorTasks = taskSource.GetActorTasks(actor);

                foreach (var actorTask in actorTasks)
                {
                    try
                    {
                        actorTask.Execute();
                    }
                    catch (Exception exception)
                    {
                        throw new ActorTaskExecutionException($"Ошибка при работе источника команд {taskSource.GetType().FullName}",
                            taskSource,
                            exception);
                    }
                }
            }
        }

        private static IActor[] CalcActorList(IActorManager actorManager)
        {
            // Персонаж, которым в данный момент управляет актёр, должен обрабатываться первым.
            var sortedActors = actorManager.Items.Where(x => !x.Person.CheckIsDead())
                .OrderByDescending(x => x.Owner is HumanPlayer)
                .ThenBy(x => x.Person.Id)
                .ToArray();

            // отсортировать по инициативе
            return sortedActors;
        }
    }
}
