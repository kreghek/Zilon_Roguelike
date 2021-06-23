using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        private void ActorManager_Removed(object? sender, ManagerItemsChangedEventArgs<IActor> e)
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
            var allStateCopyList = taskDict.Values.ToArray();
            foreach (var state in allStateCopyList)
            {
                var actor = state.Actor;

                if (state.TaskComplete)
                {
                    taskDict.Remove(actor);
                    state.TaskSource.ProcessTaskComplete(state.Task);
                    continue;
                }

                if (!actor.CanExecuteTasks)
                {
                    // Песонаж может перестать выполнять задачи по следующим причинам:
                    // 1. Персонажи, у которых есть модуль выживания, могут умереть.
                    // Мертвые персонажы не выполняют задач.
                    // Их задачи можно прервать, потому что:
                    //   * Возможна ситуация, когда мертвый персонаж все еще выполнить действие.
                    //   * Экономит ресурсы.
                    taskDict.Remove(actor);
                }
            }
        }

        private async Task GenerateActorTasksAndPutInDictAsync(IEnumerable<ActorInSector> actorDataList,
            CancellationToken cancelToken)
        {
            var actorDataListMaterialized = actorDataList.ToArray();

            var actorGrouppedBySector = actorDataListMaterialized.GroupBy(x => x.Sector).ToArray();

            foreach (var sectorGroup in actorGrouppedBySector)
            {
                foreach (var actorDataItem in sectorGroup)
                {
                    var actor = actorDataItem.Actor;
                    var sector = actorDataItem.Sector;

                    var taskSource = actor.TaskSource;

                    var context = new SectorTaskSourceContext(sector, cancelToken);

                    try
                    {
                        if (taskSource is IHumanActorTaskSource<SectorTaskSourceContext> humanTaskSource)
                        {
                            // Drop intention for human controlled task source.
                            cancelToken.Register(() => { humanTaskSource.DropIntentionWaiting(); });
                        }

                        var actorTask = await taskSource.GetActorTaskAsync(actor, context).ConfigureAwait(false);

                        var state = new TaskState(actor, sector, actorTask, taskSource);

                        _taskDict.AddOrUpdate(actor, state, (a, t) => state);
                    }
                    catch (TaskCanceledException)
                    {
                        // Do nothing for his actor. His task was cancelled.
                        _taskDict.TryRemove(actor, out var _);
                    }
                }
            }
        }

        private IEnumerable<ActorInSector> GetActorsWithoutTasks()
        {
            return GetActorsWithoutTasksEnumerator().ToArray();
        }

        private IEnumerable<ActorInSector> GetActorsWithoutTasksEnumerator()
        {
            // _sectorNodes фиксируем в отдельный массив.
            // потому что по мере выполнения задач актёров, может быть переход в новый узел мира.
            // Мир при том расширится и список поменяется. А это приведёт к ошибке, что коллекция в foreach изменяется.
            var sectorNodes = _sectorNodes.ToArray();
            foreach (var sectorNode in sectorNodes)
            {
                if (sectorNode.State != SectorNodeState.SectorMaterialized)
                {
                    // Do not process SectorNode before they are materialized.
                    // Not materialized sector nodes can't contains any actors.
                    continue;
                }

                var sector = sectorNode.Sector;
                if (sector is null)
                {
                    if (sectorNode.State == SectorNodeState.SectorMaterialized)
                    {
                        // Invalid case because sector node is materialized but sector is null.
                        throw new InvalidOperationException();
                    }

                    continue;
                }

                var actorList = sector.ActorManager.Items.ToArray();
                foreach (var actor in actorList)
                {
                    var needCreateTask = IsActorNeedsInTask(actor);

                    if (needCreateTask)
                    {
                        yield return new ActorInSector(actor, sector);
                    }
                }
            }
        }

        private static List<TaskState[]> GroupTaskStates(ICollection<TaskState> states)
        {
            var statesGroupedBySector = states.GroupBy(x => x.Sector).ToArray();
            var materializedSectorList = MaterializeSectorStates(statesGroupedBySector);
            return materializedSectorList;
        }

        private bool IsActorNeedsInTask(IActor actorInSector)
        {
            if (_taskDict.TryGetValue(actorInSector, out _))
            {
                // Actor has task yet.

                return false;
            }

            return true;
        }

        private static bool IsPlayerActorControlledByHuman(IActorTaskSource<ISectorTaskSourceContext> taskSource)
        {
            if (taskSource is IActorTaskControlSwitcher controlSwitcher)
            {
                return controlSwitcher.CurrentControl == ActorTaskSourceControl.Human;
            }

            return true;
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
                    var isPlayerActorControlledByHuman = IsPlayerActorControlledByHuman(actor.TaskSource);
                    if (isPlayerActorControlledByHuman)
                    {
                        humanTaskSource.DropIntentionWaiting();
                    }
                }
            }
        }

        private void Sector_TrasitionUsed(object? sender, TransitionUsedEventArgs e)
        {
            if (sender is null)
            {
                throw new InvalidOperationException("Invalid event sender. It must be not null.");
            }

            var sector = (ISector)sender;

            _globeTransitionHandler.InitActorTransitionAsync(this, sector, e.Actor, e.Transition).Wait(10_000);
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
            var sector = sectorNode.Sector;
            if (sector is null)
            {
                throw new InvalidOperationException();
            }

            sector.TrasitionUsed += Sector_TrasitionUsed;
            sector.ActorManager.Removed += ActorManager_Removed;
        }

        public async Task UpdateAsync(CancellationToken cancelToken)
        {
            var actorsWithoutTasks = GetActorsWithoutTasks();

            await GenerateActorTasksAndPutInDictAsync(actorsWithoutTasks, cancelToken).ConfigureAwait(false);

            ProcessTasks(_taskDict);

            _turnCounter++;

            if (_turnCounter >= GlobeMetrics.OneIterationLength)
            {
                _turnCounter = GlobeMetrics.OneIterationLength - _turnCounter;

                foreach (var sectorNode in _sectorNodes)
                {
                    var sector = sectorNode.Sector;
                    if (sector is null)
                    {
                        if (sectorNode.State == SectorNodeState.SectorMaterialized)
                        {
                            throw new InvalidOperationException();
                        }

                        continue;
                    }

                    sector.Update();
                }

                _globeTransitionHandler.UpdateTransitions();
            }
        }

        private sealed class ActorInSector
        {
            public ActorInSector(IActor actor, ISector sector)
            {
                Actor = actor;
                Sector = sector;
            }

            public IActor Actor { get; }
            public ISector Sector { get; }
        }
    }
}