using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Zilon.Bot.Players;
using Zilon.Bot.Players.Strategies;
using Zilon.Core.CommonServices;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.GlobeObserver
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            RegisterServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            LogicStateTreePatterns.Factory = serviceProvider.GetRequiredService<ILogicStateFactory>();

            var mapFactory = serviceProvider.GetRequiredService<IMapFactory>();
            var actorManager = serviceProvider.GetRequiredService<IActorManager>();
            var propContainerManager = serviceProvider.GetRequiredService<IPropContainerManager>();
            var dropResolver = serviceProvider.GetRequiredService<IDropResolver>();
            var schemeService = serviceProvider.GetRequiredService<ISchemeService>();
            var equipmentDurableService = serviceProvider.GetRequiredService<IEquipmentDurableService>();
            var humanPersonFactor = serviceProvider.GetRequiredService<IHumanPersonFactory>();
            var botPlayer = serviceProvider.GetRequiredService<IBotPlayer>();

            var actorList = await CreateGlobeAsync(mapFactory,
                                                   actorManager,
                                                   propContainerManager,
                                                   dropResolver,
                                                   schemeService,
                                                   equipmentDurableService,
                                                   humanPersonFactor,
                                                   botPlayer);

            var taskSource = serviceProvider.GetRequiredService<IActorTaskSource>();

            while (true)
            {
                NextTurn(actorList, taskSource);
                break;
            }
        }

        private static void RegisterServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IDice, LinearDice>();

            RegisterSchemeService(serviceCollection);

            serviceCollection.AddSingleton<ISurvivalRandomSource, SurvivalRandomSource>();
            serviceCollection.AddSingleton<IPropFactory, PropFactory>();
            RegisterDropResolver(serviceCollection);
            serviceCollection.AddSingleton<IHumanPersonFactory, RandomHumanPersonFactory>();

            //TODO При такой регистрации все актёры будут в одном менеджере, но в разных секторах. Это нужно перепроектировать.
            serviceCollection.AddSingleton<IActorManager, ActorManager>();
            serviceCollection.AddSingleton<IPropContainerManager, PropContainerManager>();
            RegisterEquipmentDurableService(serviceCollection);
            RegisterRoomMapFactory(serviceCollection);

            serviceCollection.AddSingleton<IUserTimeProvider, UserTimeProvider>();

            serviceCollection.AddSingleton<IBotPlayer, BotPlayer>();
            serviceCollection.AddSingleton<IActorTaskSource, MonsterBotActorTaskSource>();
            serviceCollection.AddSingleton<ILogicStateFactory, ContainerLogicStateFactory>();

            RegisterLogicState(serviceCollection);
        }

        public static void RegisterLogicState(IServiceCollection serviceRegistry)
        {
            var logicTypes = GetTypes<ILogicState>();
            var triggerTypes = GetTypes<ILogicStateTrigger>()
                .Where(x => !typeof(ICompositLogicStateTrigger).IsAssignableFrom(x));

            var allTypes = logicTypes.Union(triggerTypes);
            foreach (var logicType in allTypes)
            {
                // Регистрируем, как трансиентные. Потому что нам может потребовать несколько
                // состояний и триггеров одного и того же типа.
                // Например, для различной кастомизации.
                serviceRegistry.AddTransient(logicType);
            }
        }

        private static IEnumerable<Type> GetTypes<TInterface>()
        {
            var logicTypes = typeof(ILogicState).Assembly.GetTypes()
                .Where(x => !x.IsAbstract && !x.IsInterface && typeof(TInterface).IsAssignableFrom(x));
            return logicTypes;
        }

        private static void RegisterRoomMapFactory(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IMapFactory, RoomMapFactory>();
            serviceCollection.AddSingleton<IRoomGenerator, RoomGenerator>();
            serviceCollection.AddSingleton<IRoomGeneratorRandomSource, RoomGeneratorRandomSource>();
        }

        private static void RegisterEquipmentDurableService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IEquipmentDurableService, EquipmentDurableService>();
            serviceCollection.AddSingleton<IEquipmentDurableServiceRandomSource, EquipmentDurableServiceRandomSource>();
        }

        private static void RegisterDropResolver(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IDropResolver, DropResolver>();
            serviceCollection.AddSingleton<IDropResolverRandomSource, DropResolverRandomSource>();
        }

        private static void RegisterSchemeService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ISchemeLocator>(factory => FileSchemeLocator.CreateFromEnvVariable());
            serviceCollection.AddSingleton<ISchemeServiceHandlerFactory, SchemeServiceHandlerFactory>();
            serviceCollection.AddSingleton<ISchemeService, SchemeService>();
        }

        private static void NextTurn(IList<IActor> actorList, IActorTaskSource taskSource)
        {
            foreach (var actor in actorList)
            {
                if (actor.Person.CheckIsDead())
                {
                    continue;
                }

                ProcessActor(actor, taskSource);
            }
        }

        private static void ProcessActor(IActor actor, IActorTaskSource taskSource)
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

        private static async Task<IList<IActor>> CreateGlobeAsync(IMapFactory mapFactory,
            IActorManager actorManager,
            IPropContainerManager propContainerManager,
            IDropResolver dropResolver,
            ISchemeService schemeService,
            IEquipmentDurableService equipmentDurableService,
            IHumanPersonFactory humanPersonFactory,
            IBotPlayer botPlayer)
        {
            var actorList = new List<IActor>();

            for (var localityIndex = 0; localityIndex < 300; localityIndex++)
            {
                var localitySector = await CreateLocalitySectorAsync(mapFactory,
                                                                     actorManager,
                                                                     propContainerManager,
                                                                     dropResolver,
                                                                     schemeService,
                                                                     equipmentDurableService);

                for (var populationUnitIndex = 0; populationUnitIndex < 4; populationUnitIndex++)
                {
                    for (var personIndex = 0; personIndex < 10; personIndex++)
                    {
                        var node = localitySector.Map.Nodes.ElementAt(personIndex);
                        var person = CreatePerson(humanPersonFactory);
                        var actor = CreateActor(botPlayer, person, node, actorManager);
                        actorList.Add(actor);
                    }
                }
            }

            return actorList;
        }

        private static IPerson CreatePerson(IHumanPersonFactory humanPersonFactory)
        {
            var person = humanPersonFactory.Create();
            return person;
        }

        private static IActor CreateActor(IPlayer personPlayer,
            IPerson person,
            IMapNode node,
            IActorManager actorManager)
        {
            var actor = new Actor(person, personPlayer, node);

            actorManager.Add(actor);

            return actor;
        }

        private static async Task<ISector> CreateLocalitySectorAsync(IMapFactory mapFactory,
            IActorManager actorManager,
            IPropContainerManager propContainerManager,
            IDropResolver dropResolver,
            ISchemeService schemeService,
            IEquipmentDurableService equipmentDurableService)
        {
            var townScheme = new TownSectorScheme
            {
                RegionCount = 10,
                RegionSize = 10
            };

            var sectorMap = await mapFactory.CreateAsync(townScheme);
            var sector = new Sector(sectorMap,
                                    actorManager,
                                    propContainerManager,
                                    dropResolver,
                                    schemeService,
                                    equipmentDurableService);
            return sector;
        }
    }
}
