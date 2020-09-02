using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Zilon.Core.MapGenerators;
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

        IEnumerable<ISectorNode> SectorNodes { get; }

        Task UpdateAsync();
    }

    public interface IGlobeTransitionHandler
    {
        Task ProcessAsync(IGlobe globe, ISector sector, IActor actor, RoomTransition transition);
    }

    public sealed class GlobeTransitionHandler : IGlobeTransitionHandler
    {
        private readonly IGlobeExpander _globeExpander;

        //Instantiate a Singleton of the Semaphore with a value of 1. This means that only 1 thread can be granted access at a time.
        static readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        public GlobeTransitionHandler(IGlobeExpander globeExpander)
        {
            _globeExpander = globeExpander ?? throw new ArgumentNullException(nameof(globeExpander));
        }

        public async Task ProcessAsync(IGlobe globe, ISector sector, IActor actor, RoomTransition transition)
        {
            if (globe is null)
            {
                throw new ArgumentNullException(nameof(globe));
            }

            if (sector is null)
            {
                throw new ArgumentNullException(nameof(sector));
            }

            if (actor is null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            if (transition is null)
            {
                throw new ArgumentNullException(nameof(transition));
            }

            var sectorNode = transition.SectorNode;

            //TODO Разобраться с этим кодом.
            // https://blog.cdemi.io/async-waiting-inside-c-sharp-locks/
            //Asynchronously wait to enter the Semaphore. If no-one has been granted access to the Semaphore, code execution will proceed, otherwise this thread waits here until the semaphore is released 
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
            await _semaphoreSlim.WaitAsync();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
            try
            {
                if (sectorNode.State != SectorNodeState.SectorMaterialized)
                {
                    await _globeExpander.ExpandAsync(sectorNode).ConfigureAwait(false);
                    globe.AddSectorNode(sectorNode);
                }
            }
            finally
            {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                _semaphoreSlim.Release();
            }

            try
            {
                sector.ActorManager.Remove(actor);
            }
            catch (InvalidOperationException exception)
            {
                // Пока ничего не делаем
                Console.WriteLine(sector.GetHashCode());
                Console.WriteLine(actor);
            }

            var nextSector = sectorNode.Sector;
            var nodeForTransition = nextSector.Map.Transitions.First(x => x.Value.SectorNode.Sector == sector).Key;
            var actorInNewSector = new Actor(actor.Person, actor.TaskSource, nodeForTransition);
            nextSector.ActorManager.Add(actorInNewSector);
        }
    }

    public sealed class Globe : IGlobe
    {
        private int _turnCounter;

        private readonly IList<ISectorNode> _sectorNodes;

        private readonly ConcurrentDictionary<IActor, TaskState> _taskDict;
        private readonly IGlobeTransitionHandler _globeTransitionHandler;

        public IEnumerable<ISectorNode> SectorNodes { get => _sectorNodes; }

        public Globe(IGlobeTransitionHandler globeTransitionHandler)
        {
            _taskDict = new ConcurrentDictionary<IActor, TaskState>();

            _sectorNodes = new List<ISectorNode>();

            _globeTransitionHandler = globeTransitionHandler ?? throw new ArgumentNullException(nameof(globeTransitionHandler));
        }

        public void AddSectorNode(ISectorNode sectorNode)
        {
            if (sectorNode is null)
            {
                throw new ArgumentNullException(nameof(sectorNode));
            }

            _sectorNodes.Add(sectorNode);
            sectorNode.Sector.TrasitionUsed += Sector_TrasitionUsed;
        }

        private async void Sector_TrasitionUsed(object sender, TransitionUsedEventArgs e)
        {
            var sector = (ISector)sender;
            await _globeTransitionHandler.ProcessAsync(this, sector, e.Actor, e.Transition).ConfigureAwait(false);
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

    public interface IPersonInitializer
    {
        Task<IEnumerable<IPerson>> CreateStartPersonsAsync();
    }

    public sealed class HumanPersonInitializer : IPersonInitializer
    {
        private readonly IPersonFactory _personFactory;
        private readonly IPlayer _player;

        public HumanPersonInitializer(IPersonFactory personFactory, IPlayer player)
        {
            _personFactory = personFactory ?? throw new ArgumentNullException(nameof(personFactory));
            _player = player ?? throw new ArgumentNullException(nameof(player));
        }

        public Task<IEnumerable<IPerson>> CreateStartPersonsAsync()
        {
            var person = CreateStartPerson("human-person", _personFactory, Fractions.MainPersonFraction);
            _player.MainPerson = person;
            return Task.FromResult(new[] { person }.AsEnumerable());
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
    }

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
        private readonly IGlobeTransitionHandler _globeTransitionHandler;
        private readonly ISchemeService _schemeService;
        private readonly IActorTaskSource<ISectorTaskSourceContext> _actorTaskSource;
        private readonly IPersonInitializer _personInitializer;

        public GlobeInitializer(
            IBiomeInitializer biomeInitializer,
            IGlobeTransitionHandler globeTransitionHandler,
            ISchemeService schemeService,
            IActorTaskSource<ISectorTaskSourceContext> actorTaskSource,
            IPersonInitializer personInitializer)
        {
            _biomeInitializer = biomeInitializer;
            _globeTransitionHandler = globeTransitionHandler;
            _schemeService = schemeService;
            _actorTaskSource = actorTaskSource;
            _personInitializer = personInitializer;
        }

        public async Task<IGlobe> CreateGlobeAsync(string startLocationSchemeSid)
        {
            var globe = new Globe(_globeTransitionHandler);

            var startLocation = _schemeService.GetScheme<ILocationScheme>(startLocationSchemeSid);
            var startBiom = await _biomeInitializer.InitBiomeAsync(startLocation).ConfigureAwait(false);
            var startSectorNode = startBiom.Sectors.First(x=>x.State == SectorNodeState.SectorMaterialized);

            globe.AddSectorNode(startSectorNode);

            // Добавляем стартовых персонажей-пилигримов

            var startPersons = await _personInitializer.CreateStartPersonsAsync().ConfigureAwait(false);

            var sector = startSectorNode.Sector;
            var personCounter = 0;
            foreach (var person in startPersons)
            {
                var startNode = sector.Map
                    .Nodes
                    .Skip(personCounter)
                    .First();
                var actor = CreateActor(person, startNode, _actorTaskSource);

                sector.ActorManager.Add(actor);
                personCounter++;
            }

            return globe;
        }

        private static IActor CreateActor(
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
