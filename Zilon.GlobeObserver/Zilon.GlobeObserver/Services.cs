using System;
using System.Collections.Generic;
using System.Linq;

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
using Zilon.Core.World;
using Zilon.Core.World;

namespace Zilon.GlobeObserver
{
    public static class Services
    {
        public static void RegisterServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IDice, LinearDice>();

            RegisterSchemeService(serviceCollection);

            serviceCollection.AddSingleton<ISurvivalRandomSource, SurvivalRandomSource>();
            serviceCollection.AddSingleton<IPropFactory, PropFactory>();
            RegisterDropResolver(serviceCollection);
            serviceCollection.AddSingleton<IHumanPersonFactory, RandomHumanPersonFactory>();

            serviceCollection.AddScoped<IActorManager, ActorManager>();
            serviceCollection.AddScoped<IPropContainerManager, PropContainerManager>();
            RegisterEquipmentDurableService(serviceCollection);
            RegisterRoomMapFactory(serviceCollection);

            serviceCollection.AddSingleton<IUserTimeProvider, UserTimeProvider>();

            RegisterBotTaskSource(serviceCollection);

            serviceCollection.AddScoped<ISectorManager, GenerationSectorManager>();

            RegisterGlobeServices(serviceCollection);

            RegisterStorageService(serviceCollection);
        }

        private static void RegisterStorageService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<GlobeStorage>();
            serviceCollection.AddScoped<ISectorInfoFactory, SectorInfoFactory>();
        }

        private static void RegisterGlobeServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<TerrainInitiator>();
            serviceCollection.AddSingleton<ProvinceInitiator>();
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
    }
}
