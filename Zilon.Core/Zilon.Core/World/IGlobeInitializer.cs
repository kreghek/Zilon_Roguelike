using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zilon.Core.PersonGeneration;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.World
{
    public interface IGlobeInitializer
    {
        Task<IGlobe> CreateGlobeAsync(string startLocationSchemeSid);
    }

    public interface IGlobe
    {
        void AddSectorNode(ISectorNode sectorNode);
        Task UpdateAsync();
    }

    public sealed class Globe : IGlobe
    {
        private int _turnCounter;

        private readonly IList<ISectorNode> _sectorNodes;

        private readonly ConcurrentDictionary<IActor, TaskState> _taskDict;

        public Globe()
        {
            _taskDict = new ConcurrentDictionary<IActor, TaskState>();
            _sectorNodes = new List<ISectorNode>();
        }

        public IEnumerable<ISectorNode> SectorNodes { get; }

        public void AddSectorNode(ISectorNode sectorNode)
        {
            _sectorNodes.Add(sectorNode);
        }

        public async Task UpdateAsync()
        {
            var actorsWithoutTasks = GetActorsWithoutTasks();

            await GenerateActorTasksAndPutInDictAsync(actorsWithoutTasks).ConfigureAwait(false);

            ProcessTasks(_taskDict);
            _turnCounter++;
            if (_turnCounter >= 1000)
            {
                _turnCounter = 1000 - _turnCounter;

                foreach (var sectorNode in SectorNodes)
                {
                    sectorNode.Sector.Update();
                }
            }
        }

        private IEnumerable<IActor> GetActorsWithoutTasks()
        {
            foreach (var sectorNode in _sectorNodes)
            {
                foreach (var actor in sectorNode.Sector.ActorManager.Items)
                {
                    if (_taskDict.TryGetValue(actor, out _))
                    {
                        continue;
                    }

                    yield return actor;
                }
            }
        }

        private async Task GenerateActorTasksAndPutInDictAsync(IEnumerable<IActor> actors)
        {
            foreach (var actor in actors)
            {
                var taskSource = actor.TaskSource;

                var actorTask = await taskSource.GetActorTaskAsync(actor).ConfigureAwait(false);

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
    }

    public sealed class GlobeInitializer : IGlobeInitializer
    {
        private readonly IBiomeInitializer _biomeInitializer;
        private readonly ISchemeService _schemeService;
        private readonly IPersonFactory _personFactory;
        private readonly IActorTaskSource _actorTaskSource;

        public GlobeInitializer(IBiomeInitializer biomeInitializer, ISchemeService schemeService, IPersonFactory personFactory, IActorTaskSource actorTaskSource)
        {
            _biomeInitializer = biomeInitializer;
            _schemeService = schemeService;
            _personFactory = personFactory;
            _actorTaskSource = actorTaskSource;
        }

        public async Task<IGlobe> CreateGlobeAsync(string startLocationSchemeSid)
        {
            var globe = new Globe();

            var startLocation = _schemeService.GetScheme<ILocationScheme>(startLocationSchemeSid);
            var startBiom = await _biomeInitializer.InitBiomeAsync(startLocation).ConfigureAwait(false);
            var startSectorNode = startBiom.Sectors.First(x=>x.State == SectorNodeState.SectorMaterialized);

            globe.AddSectorNode(startSectorNode);

            // Добавляем стартовых персонажей-пилигримов
            var sector = startSectorNode.Sector;
            for (var i = 0; i < 40; i++)
            {
                var person = CreateStartPerson("human-person", _personFactory);

                var startNode = sector.Map.Regions.SingleOrDefault(x => x.IsStart)
                    .Nodes
                    .Skip(i)
                    .First();
                var actor = CreateHumanActor(person, startNode, _actorTaskSource);

                sector.ActorManager.Add(actor);
            }

            return globe;
        }

        private static IActor CreateHumanActor(
            IPerson humanPerson,
            Graphs.IGraphNode startNode,
            IActorTaskSource actorTaskSource)
        {
            var actor = new Actor(humanPerson, actorTaskSource, startNode);

            return actor;
        }

        /// <summary>
        /// Создаёт персонажа.
        /// </summary>
        /// <param name="serviceProvider"> Контейнер DI, откуда извлекаются сервисы для создания персонажа. </param>
        /// <returns> Возвращает созданного персонажа. </returns>
        private static IPerson CreateStartPerson(string personSchemeSid, IPersonFactory personFactory)
        {
            var startPerson = personFactory.Create(personSchemeSid);
            return startPerson;
        }
    }

    public interface IGlobeService
    {
        Task ExpandGlobeAsync(IGlobe globe, ISectorNode sectorNode);
    }

    public sealed class GlobeService : IGlobeService
    {
        private readonly IBiomeInitializer _biomeInitializer;

        public GlobeService(IBiomeInitializer biomeInitializer)
        {
            _biomeInitializer = biomeInitializer;
        }

        public async Task ExpandGlobeAsync(IGlobe globe, ISectorNode sectorNode)
        {
            if (globe is null)
            {
                throw new ArgumentNullException(nameof(globe));
            }

            if (sectorNode is null)
            {
                throw new ArgumentNullException(nameof(sectorNode));
            }

            if (sectorNode.State == SectorNodeState.SectorMaterialized)
            {
                throw new InvalidOperationException("В этом случае такой узел должен уже быть использован.");
            }

            await _biomeInitializer.MaterializeLevelAsync(sectorNode).ConfigureAwait(false);

            // Фиксируем новый узел, как известную, материализованную часть мира.
            // Далее этот узел будет обрабатываться при каждом изменении мира.
            globe.AddSectorNode(sectorNode);
        }
    }
}
