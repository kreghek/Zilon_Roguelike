using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.PersonModules;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Tactics
{
    public sealed class GameLoop : IGameLoop
    {
        private readonly ConcurrentDictionary<IActor, TaskState> _taskDict;

        private readonly ISectorManager _sectorManager;
        private readonly IActorTaskSourceCollector _actorTaskSourceCollector;
        private int _turnCounter;

        public event EventHandler Updated;

        public GameLoop(ISectorManager sectorManager, IActorTaskSourceCollector actorTaskSourceCollector)
        {
            _taskDict = new ConcurrentDictionary<IActor, TaskState>();

            _sectorManager = sectorManager ?? throw new ArgumentNullException(nameof(sectorManager));
            _actorTaskSourceCollector = actorTaskSourceCollector ?? throw new ArgumentNullException(nameof(actorTaskSourceCollector));
        }

        public async Task UpdateAsync()
        {
            var actorsWithoutTasks = GetActorsWithoutTasks(_sectorManager.CurrentSector.ActorManager, _taskDict);
            await GenerateActorTasksAndPutInDictAsync(actorsWithoutTasks, _taskDict).ConfigureAwait(false);

            ProcessTasks(_taskDict);
            _turnCounter++;
            if (_turnCounter >= 1000)
            {
                _turnCounter = 1000 - _turnCounter;

                _sectorManager.CurrentSector.Update();

                Updated?.Invoke(this, new EventArgs());
            }
        }

        private static void ProcessTasks(IDictionary<IActor, TaskState> taskDict)
        {
            var states = taskDict.Values;
            foreach (var state in states)
            {
                var survivalModule = state.Actor.Person.GetModuleSafe<ISurvivalModule>();
                if (survivalModule?.IsDead == true)
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
                    state.TaskSource.ProcessTaskExecuted(state.Task);
                }
            }

            // удаляем выполненные задачи.
            foreach (var taskStatePair in taskDict.ToArray())
            {
                var state = taskStatePair.Value;
                if (state.TaskComplete)
                {
                    taskDict.Remove(taskStatePair.Key);
                    state.TaskSource.ProcessTaskComplete(state.Task);
                }

                var person = taskStatePair.Key.Person;
                var survivalModule = person.GetModuleSafe<ISurvivalModule>();
                if (survivalModule?.IsDead == true)
                {
                    // Персонажи, у которых есть модуль выживания, могут умереть.
                    // Мертвые персонажы не выполняют задач.
                    // Их задачи можно прервать, потому что:
                    // 1. Возможна ситуация, когда мертвый персонаж всё еще выполнить действие.
                    // 2. Экономит ресурсы.
                    taskDict.Remove(taskStatePair.Key);
                }
            }
        }

        private async Task GenerateActorTasksAndPutInDictAsync(IEnumerable<IActor> actors, ConcurrentDictionary<IActor, TaskState> taskDict)
        {
            var taskSources = _actorTaskSourceCollector.GetCurrentTaskSources();

            foreach (var actor in actors)
            {
                foreach (var taskSource in taskSources)
                {
                    if (!taskSource.CanGetTask(actor))
                    {
                        continue;
                    }

                    var actorTask = await taskSource.GetActorTaskAsync(actor).ConfigureAwait(false);

                    var state = new TaskState(actor, actorTask, taskSource);
                    if (!taskDict.TryAdd(actor, state))
                    {
                        // Это происходит, когда игрок пытается присвоить новую команду,
                        // когда старая еще не закончена и не может быть заменена.
                        throw new InvalidOperationException("Попытка назначить задачу, когда старая еще не удалена.");
                    }
                }
            }
        }

        private static IEnumerable<IActor> GetActorsWithoutTasks(IActorManager actorManager, IDictionary<IActor, TaskState> taskDict)
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
