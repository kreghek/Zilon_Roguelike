using System;
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

        private Dictionary<IActor, TaskState> _taskDict;

        private readonly ISectorManager _sectorManager;

        public event EventHandler Updated;

        public GameLoop(ISectorManager sectorManager)
        {
            _sectorManager = sectorManager;

            _taskDict = new Dictionary<IActor, TaskState>();
        }

        public IActorTaskSource[] ActorTaskSources { get; set; }

        public async Task UpdateAsync()
        {
            if (ActorTaskSources is null)
            {
                throw new InvalidOperationException("Не заданы источники команд");
            }

            // Алгоритм:
            // В игровом цикле 1000 итераций. На каждой итерации проверяем, какой актёр еще ничего не делает.
            // Если актёр ничего не делает, то запрашиваем для него задачу. Задачу запоминаем.
            // Каждую итерацию существующие задачи имеют свой счётчик. Задача выполняется, когда счётчик достигнет значения выполнения.
            // Задача на актёра убирается, когда счётчик задачи достигнет 0.
            // После выполнения 1000 итераций переход к следующему игровому циклу - обновление голода, жажды, сектора и т.д.

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

        private void ProcessTasks(Dictionary<IActor, TaskState> taskDict)
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

        private async Task GenerateActorTasksAndPutInDictAsync(IEnumerable<IActor> actors, Dictionary<IActor, TaskState> taskDict)
        {
            foreach (var actor in actors)
            {
                foreach (var taskSource in ActorTaskSources)
                {
                    if (!taskSource.CanGetTask(actor))
                    {
                        continue;
                    }

                    var task = await taskSource.GetActorTaskAsync(actor).ConfigureAwait(false);

                    var state = new TaskState(actor, task);
                    taskDict.Add(actor, state);
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
    }
}
