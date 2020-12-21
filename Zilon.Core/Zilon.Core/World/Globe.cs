using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.World
{
    /// <summary>
    /// Globe base implementation.
    /// </summary>
    public sealed class Globe : IGlobe
    {
        private readonly IGlobeTransitionHandler _globeTransitionHandler;

        private readonly IList<ISectorNode> _sectorNodes;

        private readonly ConcurrentDictionary<IActor, TaskState> _taskDict;
        private int _turnCounter;

        public Globe(IGlobeTransitionHandler globeTransitionHandler)
        {
            _taskDict = new ConcurrentDictionary<IActor, TaskState>();

            _sectorNodes = new List<ISectorNode>();

            _globeTransitionHandler =
                globeTransitionHandler ?? throw new ArgumentNullException(nameof(globeTransitionHandler));
        }

        private void ActorManager_Removed(object sender, ManagerItemsChangedEventArgs<IActor> e)
        {
            // Remove all current tasks
            foreach (var removedActor in e.Items)
            {
                RemoveActorTaskState(removedActor);
            }
        }

        private static void ClearUpTasks(IDictionary<IActor, TaskState> taskDict)
        {
            // удаляем выполненные задачи.
            foreach (var taskStatePair in taskDict.ToArray())
            {
                var state = taskStatePair.Value;

                if (state.TaskComplete)
                {
                    taskDict.Remove(taskStatePair.Key);
                    state.TaskSource.ProcessTaskComplete(state.Task);
                    return;
                }

                var actor = taskStatePair.Key;
                if (!actor.CanExecuteTasks)
                {
                    // Песонаж может перестать выполнять задачи по следующим причинам:
                    // 1. Персонажи, у которых есть модуль выживания, могут умереть.
                    // Мертвые персонажы не выполняют задач.
                    // Их задачи можно прервать, потому что:
                    //   * Возможна ситуация, когда мертвый персонаж все еще выполнить действие.
                    //   * Экономит ресурсы.
                    taskDict.Remove(taskStatePair.Key);
                }
            }
        }

        private void GenerateActorTasksAndPutInDict(IEnumerable<ActorInSector> actorDataList)
        {
            var actorDataListMaterialized = actorDataList.ToArray();

            var actorGrouppedBySector = actorDataListMaterialized.GroupBy(x => x.Sector).ToArray();

            Parallel.ForEach(actorGrouppedBySector, async sectorGroup =>
            {
                foreach (var actorDataItem in sectorGroup)
                {
                    var actor = actorDataItem.Actor;
                    var sector = actorDataItem.Sector;

                    var taskSource = actor.TaskSource;

                    var context = new SectorTaskSourceContext(sector);

                    var actorTaskTask = taskSource.GetActorTaskAsync(actor, context);

                    try
                    {
                        var actorTask = await actorTaskTask.ConfigureAwait(false);
                        var state = new TaskState(actor, sector, actorTask, taskSource);
                        if (!_taskDict.TryAdd(actor, state))
                        {
                            // Это происходит, когда игрок пытается присвоить новую команду,
                            // когда старая еще не закончена и не может быть заменена.
                            throw new InvalidOperationException(
                                "Попытка назначить задачу, когда старая еще не удалена.");
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        // Do nothing for his actor. His task was cancelled.
                        _taskDict.TryRemove(actor, out var _);
                    }
                }
            });
        }

        private IEnumerable<ActorInSector> GetActorsWithoutTasks()
        {
            // _sectorNodes фиксируем в отдельный массив.
            // потому что по мере выполнения задач актёров, может быть переход в новый узел мира.
            // Мир при том расширится и список поменяется. А это приведёт к ошибке, что коллекция в foreach изменяется.
            foreach (var sectorNode in _sectorNodes.ToArray())
            {
                var sector = sectorNode.Sector;
                foreach (var actor in sector.ActorManager.Items.ToArray())
                {
                    if (!sector.ActorManager.Items.Contains(actor))
                    {
                        // Это может произойти, когда в процессе исполнения задач,
                        // актёр вышел из сектора. Соответственно, его уже нет в списке актёров сектора.

                        RemoveActorTaskState(actor);

                        continue;
                    }

                    if (_taskDict.TryGetValue(actor, out _))
                    {
                        continue;
                    }

                    yield return new ActorInSector { Actor = actor, Sector = sector };
                }
            }
        }

        private static List<TaskState[]> GroupTaskStates(ICollection<TaskState> states)
        {
            var statesGroupedBySector = states.GroupBy(x => x.Sector).ToArray();
            var materializedSectorList = MaterializeSectorStates(statesGroupedBySector);
            return materializedSectorList;
        }

        private static List<TaskState[]> MaterializeSectorStates(IGrouping<ISector, TaskState>[] statesGroupedBySector)
        {
            var materializedSectorList = new List<TaskState[]>();
            foreach (var sectorStates in statesGroupedBySector)
            {
                materializedSectorList.Add(sectorStates.ToArray());
            }

            return materializedSectorList;
        }

        private static void ProcessActorStates(IEnumerable<TaskState> states)
        {
            foreach (var state in states)
            {
                if (!state.Actor.CanExecuteTasks)
                {
                    // По сути, возможность выполнять задачи - это основая роль этого объекта.
                    // песонаж может перестать выполнять задачи по следующим причинам:
                    // 1. Персонажи, у которых есть модуль выживания, могут умереть, пока выполняют текущую задачу (выполняются предусловия).
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
        }

        private static void ProcessTasks(IDictionary<IActor, TaskState> taskDict)
        {
            var states = taskDict.Values;
            var materializedSectorList = GroupTaskStates(states);

            foreach (var sectorStates in materializedSectorList)
            {
                var orderedStates = SortActorStates(sectorStates);
                ProcessActorStates(orderedStates);
            }

            ClearUpTasks(taskDict);
        }

        private void RemoveActorTaskState(IActor actor)
        {
            if (_taskDict.TryRemove(actor, out var taskState))
            {
                taskState.TaskSource.CancelTask(taskState.Task);
            }
            else
            {
                if (actor.TaskSource is IHumanActorTaskSource<ISectorTaskSourceContext> humanTaskSource)
                {
                    humanTaskSource.DropIntentionWaiting();
                }
            }
        }

        private async void Sector_TrasitionUsed(object sender, TransitionUsedEventArgs e)
        {
            var sector = (ISector)sender;
            await _globeTransitionHandler.InitActorTransitionAsync(this, sector, e.Actor, e.Transition)
                .ConfigureAwait(false);
        }

        private static TaskState[] SortActorStates(IEnumerable<TaskState> sectorStates)
        {
            // Все актёры еще раз сортируются, чтобы зафиксировать поведение для тестов.
            // Может быть ситуация, когда найдено одновременно два актёра без задач (в самом начале теста, например).
            // В этом случае порядок элементов, полученных из states, не фиксирован.
            // В тестах мжет быть важно, в каком порядке ходят актёры, чтобы корректно обрабатывать коллизии.
            // Поэтому тут выполняется еще одна сортировка, от которой, по-хорошему нужно избавиться.
            // Лучше сразу иметь список states с сортировкой по приоритету при добавлении элемента.

            return sectorStates.OrderBy(x => x.Actor.Person.Id).ToArray();
        }

        public IEnumerable<ISectorNode> SectorNodes => _sectorNodes;

        public void AddSectorNode(ISectorNode sectorNode)
        {
            if (sectorNode is null)
            {
                throw new ArgumentNullException(nameof(sectorNode));
            }

            _sectorNodes.Add(sectorNode);
            sectorNode.Sector.TrasitionUsed += Sector_TrasitionUsed;
            sectorNode.Sector.ActorManager.Removed += ActorManager_Removed;
        }

        public Task UpdateAsync()
        {
            return Task.Run(() =>
            {
                var actorsWithoutTasks = GetActorsWithoutTasks();

                GenerateActorTasksAndPutInDict(actorsWithoutTasks);

                ProcessTasks(_taskDict);
                _turnCounter++;
                if (_turnCounter >= GlobeMetrics.OneIterationLength)
                {
                    _turnCounter = GlobeMetrics.OneIterationLength - _turnCounter;

                    foreach (var sectorNode in _sectorNodes)
                    {
                        sectorNode.Sector.Update();
                    }

                    _globeTransitionHandler.UpdateTransitions();
                }
            });
        }

        private sealed class ActorInSector
        {
            public IActor Actor { get; set; }
            public ISector Sector { get; set; }
        }
    }
}