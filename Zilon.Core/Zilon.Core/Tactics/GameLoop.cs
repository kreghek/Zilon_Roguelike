using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zilon.Core.PersonModules;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Tactics
{
    public sealed partial class GameLoop : IGameLoop
    {
        private int _turnCounter;

        private ConcurrentDictionary<IActor, TaskState> _taskDict;

        private readonly ISectorManager _sectorManager;

        public event EventHandler Updated;

        public GameLoop(ISectorManager sectorManager)
        {
            _sectorManager = sectorManager;

            _taskDict = new ConcurrentDictionary<IActor, TaskState>();
        }

        public IActorTaskSource[] ActorTaskSources { get; set; }

        public IEnumerable<IActor> Update()
        {
            if (ActorTaskSources is null)
            {
                throw new InvalidOperationException("Не заданы источники команд");
            }

            // Алгоритм:
            // Выполняем итерацию.
            // Сначала в отдельном публичном методе создаём задачи персонажам.
            // Возврат - персонажи, у которых нет задач (сейчас это один персонаж).

            // В этом методе выполняем задачи.

            var actorsWithoutTasks = GetActorsWithoutTasks(_sectorManager.CurrentSector.ActorManager, _taskDict);
            GenerateActorTasksAndPutInDict(actorsWithoutTasks, _taskDict);
            var actorsWithoutTasksAfterGeneration = GetActorsWithoutTasks(_sectorManager.CurrentSector.ActorManager, _taskDict);
            if (actorsWithoutTasksAfterGeneration.Any())
            {
                return actorsWithoutTasksAfterGeneration;
            }

            ProcessTasks(_taskDict);
            _turnCounter++;
            if (_turnCounter >= 1000)
            {
                _turnCounter = 1000 - _turnCounter;

                _sectorManager.CurrentSector.Update();

                Updated?.Invoke(this, new EventArgs());
            }

            return Array.Empty<IActor>();
        }

        private void ProcessTasks(IDictionary<IActor, TaskState> taskDict)
        {
            var states = taskDict.Values;
            foreach (var state in states)
            {
                if (state.Actor.Person.GetModuleSafe<ISurvivalModule>()?.IsDead == true)
                {
                    // Персонажи, у которых есть модуль выживания, могут умереть.
                    // Мертвые персонажы не выполняют задач.
                    // Их задачи игнорируем, т.к. задачи могут выполниться после смерти персонажа,
                    // что противоречит логике задач.
                    continue;
                }

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
                    continue;
                }

                if (taskStatePair.Key.Person.GetModuleSafe<ISurvivalModule>()?.IsDead == true)
                {
                    // Персонажи, у которых есть модуль выживания, могут умереть.
                    // Мертвые персонажы не выполняют задач.
                    // Их задачи можно прервать, потому что:
                    // 1. Возможна ситуация, когда мертвый персонаж всё еще выполнить действие.
                    // 2. Экономит ресурсы.
                    taskDict.Remove(taskStatePair.Key);
                    continue;
                }
            }
        }

        private void GenerateActorTasksAndPutInDict(IEnumerable<IActor> actors, ConcurrentDictionary<IActor, TaskState> taskDict)
        {
            Parallel.ForEach(actors, async (actor) =>
            {
                foreach (var taskSource in ActorTaskSources)
                {
                    if (!taskSource.CanGetTask(actor))
                    {
                        continue;
                    }

                    var task = await taskSource.GetActorTaskAsync(actor).ConfigureAwait(false);

                    var state = new TaskState(actor, task);
                    if (!taskDict.TryAdd(actor, state))
                    {
                        throw new Exception();
                    }
                }
            });
        }

        private IEnumerable<IActor> GetActorsWithoutTasks(IActorManager actorManager, IDictionary<IActor, TaskState> taskDict)
        {
            foreach (var actor in actorManager.Items)
            {
                if (taskDict.TryGetValue(actor, out _))
                {
                    continue;
                }

                yield return actor;
            }
        }
    }
}
