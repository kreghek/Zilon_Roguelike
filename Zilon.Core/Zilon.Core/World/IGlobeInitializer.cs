using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.PersonGeneration;
using Zilon.Core.Persons;
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

        IEnumerable<ISectorNode> SectorNodes { get; }

        Task UpdateAsync();
    }

    public sealed class Globe : IGlobe
    {
        private int _turnCounter;

        private readonly IList<ISectorNode> _sectorNodes;

        private readonly ConcurrentDictionary<IActor, TaskState> _taskDict;

        public IEnumerable<ISectorNode> SectorNodes { get => _sectorNodes; }

        public Globe()
        {
            _taskDict = new ConcurrentDictionary<IActor, TaskState>();
            _sectorNodes = new List<ISectorNode>();
        }

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

                foreach (var sectorNode in _sectorNodes)
                {
                    sectorNode.Sector.Update();
                }
            }
        }

        private IEnumerable<ActorInSector> GetActorsWithoutTasks()
        {
            foreach (var sectorNode in _sectorNodes)
            {
                var sector = sectorNode.Sector;
                foreach (var actor in sector.ActorManager.Items)
                {
                    if (_taskDict.TryGetValue(actor, out _))
                    {
                        continue;
                    }

                    yield return new ActorInSector { Actor = actor, Sector = sector };
                }
            }
        }

        private async Task GenerateActorTasksAndPutInDictAsync(IEnumerable<ActorInSector> actorDataList)
        {
            foreach (var actorDataItem in actorDataList)
            {
                var actor = actorDataItem.Actor;
                var sector = actorDataItem.Sector;

                var taskSource = actor.TaskSource;

                var context = new SectorTaskSourceContext(sector);

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

        private sealed class ActorInSector
        {
            public ISector Sector { get; set; }
            public IActor Actor { get; set; }
        }
    }

    public interface IPersonInitializer
    {
        Task<IEnumerable<IPerson>> CreateStartPersonsAsync();
    }

    public sealed class HumanPersonInitializer: 

    public sealed class AutoPersonInitializer : IPersonInitializer
    {
        private readonly IFraction _pilgrimFraction;
        private readonly IPersonFactory _personFactory;

        public AutoPersonInitializer(IPersonFactory personFactory)
        {
            _pilgrimFraction = new Fraction("Pilgrims");

            _personFactory = personFactory;
        }

        public IEnumerable<IPerson> CreateStartPersonsInner()
        {
            for (var i = 0; i < 40; i++)
            {
                yield return CreateStartPerson("human-person", _personFactory, _pilgrimFraction);
            }
        }

        /// <summary>
        /// Создаёт персонажа.
        /// </summary>
        /// <returns> Возвращает созданного персонажа. </returns>
        private static IPerson CreateStartPerson(string personSchemeSid, IPersonFactory personFactory, IFraction fraction)
        {
            var startPerson = personFactory.Create(personSchemeSid, fraction);
            return startPerson;
        }

        public Task<IEnumerable<IPerson>> CreateStartPersonsAsync()
        {
            return Task.FromResult(CreateStartPersonsInner());
        }
    }

    public sealed class GlobeInitializer : IGlobeInitializer
    {
        private readonly IBiomeInitializer _biomeInitializer;
        private readonly ISchemeService _schemeService;
        private readonly IActorTaskSource<ISectorTaskSourceContext> _actorTaskSource;
        private readonly IPersonInitializer _personInitializer;

        public GlobeInitializer(
            IBiomeInitializer biomeInitializer,
            ISchemeService schemeService,
            IActorTaskSource<ISectorTaskSourceContext> actorTaskSource,
            IPersonInitializer personInitializer)
        {
            _biomeInitializer = biomeInitializer;
            _schemeService = schemeService;
            _actorTaskSource = actorTaskSource;
            _personInitializer = personInitializer;
        }

        public async Task<IGlobe> CreateGlobeAsync(string startLocationSchemeSid)
        {
            var globe = new Globe();

            var startLocation = _schemeService.GetScheme<ILocationScheme>(startLocationSchemeSid);
            var startBiom = await _biomeInitializer.InitBiomeAsync(startLocation).ConfigureAwait(false);
            var startSectorNode = startBiom.Sectors.First(x=>x.State == SectorNodeState.SectorMaterialized);

            globe.AddSectorNode(startSectorNode);

            // Добавляем стартовых персонажей-пилигримов

            var startPersons = await _personInitializer.CreateStartPersonsAsync().ConfigureAwait(false);

            var sector = startSectorNode.Sector;
            var startPilgrimFraction = new Fraction("Pilgrims");
            var personCounter = 0;
            foreach (var person in startPersons)
            {
                var startNode = sector.Map
                    .Nodes
                    .Skip(personCounter)
                    .First();
                var actor = CreateHumanActor(person, startNode, _actorTaskSource);

                sector.ActorManager.Add(actor);
                personCounter++;
            }

            return globe;
        }

        private static IActor CreateHumanActor(
            IPerson humanPerson,
            Graphs.IGraphNode startNode,
            IActorTaskSource<ISectorTaskSourceContext> actorTaskSource)
        {
            var actor = new Actor(humanPerson, actorTaskSource, startNode);

            return actor;
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
