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

        public async Task UpdateAsync()
        {
            var actorsWithoutTasks = GetActorsWithoutTasks();

            await GenerateActorTasksAndPutInDictAsync(actorsWithoutTasks).ConfigureAwait(false);

            ProcessTasks(_taskDict);
            _turnCounter++;
            if (_turnCounter >= GlobeMetrics.OneIterationLength)
            {
                _turnCounter = GlobeMetrics.OneIterationLength - _turnCounter;

                foreach (var sectorNode in _sectorNodes)
                {
                    sectorNode.Sector.Update();
                }
            }
        }

        private void ActorManager_Removed(object sender, ManagerItemsChangedEventArgs<IActor> e)
        {
            // Remove all current tasks
            foreach (var removedActor in e.Items)
            {
                RemoveActorTaskState(removedActor);
            }
        }

        private void RemoveActorTaskState(IActor actor)
        {
            if (_taskDict.TryRemove(actor, out var taskState))
            {
                taskState.TaskSource.CancelTask(taskState.Task);
            }
        }

        private async void Sector_TrasitionUsed(object sender, TransitionUsedEventArgs e)
        {
            var sector = (ISector)sender;
            await _globeTransitionHandler.ProcessAsync(this, sector, e.Actor, e.Transition).ConfigureAwait(false);
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

                    yield return new ActorInSector {Actor = actor, Sector = sector};
                }
            }
        }

        private async Task GenerateActorTasksAndPutInDictAsync(IEnumerable<ActorInSector> actorDataList)
        {
            var actorDataListMaterialized = actorDataList.ToArray();
            foreach (var actorDataItem in actorDataListMaterialized)
            {
                var actor = actorDataItem.Actor;
                var sector = actorDataItem.Sector;

                var taskSource = actor.TaskSource;

                var context = new SectorTaskSourceContext(sector);

                //TODO Это можно делать параллельно. Одновременно должны думать сразу все актёры.
                // Делается легко, через Parallel.ForEach, но из-за этого часто заваливаются тесты и даже не видно причины отказа.
                var actorTask = await taskSource.GetActorTaskAsync(actor, context).ConfigureAwait(false);

                var state = new TaskState(actor, actorTask, taskSource);
                if (!_taskDict.TryAdd(actor, state))
                {
                    // Это происходит, когда игрок пытается присвоить новую команду,
                    // когда старая еще не закончена и не может быть заменена.
                    throw new InvalidOperationException("Попытка назначить задачу, когда старая еще не удалена.");
                }
            }
        }

        private static void ProcessTasks(IDictionary<IActor, TaskState> taskDict)
        {
            var states = taskDict.Values;
            // Все актёры еще раз сортируются, чтобы зафиксировать поведение для тестов.
            // Может быть ситуация, когда найдено одновременно два актёра без задач (в самом начале теста, например).
            // В этом случае порядок элементов, полученных из states, не фиксирован.
            // В тестах мжет быть важно, в каком порядке ходят актёры, чтобы корректно обрабатывать коллизии.
            // Поэтому тут выполняется еще одна сортировка, от которой, по-хорошему нужно избавиться.
            // Лучше сразу иметь список states с сортировкой по приоритету при добавлении элемента.
            var orderedStates = states.OrderBy(x => x.Actor.Person.Id);
            foreach (var state in orderedStates)
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

            // удаляем выполненные задачи.
            foreach (var taskStatePair in taskDict.ToArray())
            {
                var state = taskStatePair.Value;
                if (state.TaskComplete)
                {
                    taskDict.Remove(taskStatePair.Key);
                    state.TaskSource.ProcessTaskComplete(state.Task);
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

        private sealed class ActorInSector
        {
            public ISector Sector { get; set; }
            public IActor Actor { get; set; }
        }
    }
}