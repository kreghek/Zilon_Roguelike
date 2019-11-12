using System;
using System.Collections.Concurrent;
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
using Zilon.Core.Tactics.Behaviour.Bots;
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

            var scopes = await CreateGlobeAsync(serviceProvider);

            var iteraion = 0;

            while (iteraion < 40000)
            {
                Parallel.ForEach(scopes, scope =>
                {
                    var taskSource = scope.ServiceProvider.GetRequiredService<IActorTaskSource>();
                    var actorManager = scope.ServiceProvider.GetRequiredService<IActorManager>();
                    NextTurn(actorManager, taskSource);
                });

                iteraion++;
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
            serviceCollection.AddScoped<IActorManager, ActorManager>();
            serviceCollection.AddScoped<IPropContainerManager, PropContainerManager>();
            RegisterEquipmentDurableService(serviceCollection);
            RegisterRoomMapFactory(serviceCollection);

            serviceCollection.AddSingleton<IUserTimeProvider, UserTimeProvider>();

            RegisterBotTaskSource(serviceCollection);

            serviceCollection.AddScoped<ISectorManager, GenerationSectorManager>();
        }

        private static void RegisterBotTaskSource(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IBotPlayer, BotPlayer>();

            serviceCollection.AddSingleton<IDecisionSource, DecisionSource>();

            serviceCollection.AddScoped<IActorTaskSource, MonsterBotActorTaskSource>();
            serviceCollection.AddScoped<ILogicStateFactory, ContainerLogicStateFactory>();

            RegisterLogicState(serviceCollection);

            serviceCollection.AddScoped<LogicStateTreePatterns>();
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

        private static void NextTurn(IActorManager actors, IActorTaskSource taskSource)
        {
            foreach (var actor in actors.Items)
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

        private static Task<IList<IServiceScope>> CreateGlobeAsync(IServiceProvider serviceProvider)
        {
            return Task.Run(() =>
            {
                var scopesList = new ConcurrentBag<IServiceScope>();

                Parallel.For(0, 300, async localityIndex =>
                {
                    var scope = serviceProvider.CreateScope();

                    var mapFactory = scope.ServiceProvider.GetRequiredService<IMapFactory>();
                    var actorManager = scope.ServiceProvider.GetRequiredService<IActorManager>();
                    var propContainerManager = scope.ServiceProvider.GetRequiredService<IPropContainerManager>();
                    var dropResolver = scope.ServiceProvider.GetRequiredService<IDropResolver>();
                    var schemeService = scope.ServiceProvider.GetRequiredService<ISchemeService>();
                    var equipmentDurableService = scope.ServiceProvider.GetRequiredService<IEquipmentDurableService>();
                    var humanPersonFactory = scope.ServiceProvider.GetRequiredService<IHumanPersonFactory>();
                    var botPlayer = scope.ServiceProvider.GetRequiredService<IBotPlayer>();

                    var localitySector = await CreateLocalitySectorAsync(mapFactory,
                                                                         actorManager,
                                                                         propContainerManager,
                                                                         dropResolver,
                                                                         schemeService,
                                                                         equipmentDurableService);

                    var sectorManager = scope.ServiceProvider.GetRequiredService<ISectorManager>();
                    (sectorManager as GenerationSectorManager).CurrentSector = localitySector;

                    for (var populationUnitIndex = 0; populationUnitIndex < 4; populationUnitIndex++)
                    {
                        for (var personIndex = 0; personIndex < 10; personIndex++)
                        {
                            var node = localitySector.Map.Nodes.ElementAt(personIndex);
                            var person = CreatePerson(humanPersonFactory);
                            var actor = CreateActor(botPlayer, person, node);
                            actorManager.Add(actor);
                        }
                    }

                    scopesList.Add(scope);
                });

                return scopesList.ToList() as IList<IServiceScope>;
            });
        }

        private static IPerson CreatePerson(IHumanPersonFactory humanPersonFactory)
        {
            var person = humanPersonFactory.Create();
            return person;
        }

        private static IActor CreateActor(IPlayer personPlayer,
            IPerson person,
            IMapNode node)
        {
            var actor = new Actor(person, personPlayer, node);

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
