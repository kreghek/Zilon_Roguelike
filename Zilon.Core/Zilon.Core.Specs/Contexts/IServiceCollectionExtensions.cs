using Microsoft.Extensions.DependencyInjection;

using Zilon.Bot.Players;
using Zilon.Bot.Players.NetCore;
using Zilon.Core.Client;
using Zilon.Core.Client.Sector;
using Zilon.Core.Commands;
using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.PersonGeneration;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Scoring;
using Zilon.Core.Specs.Mocks;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.World;
using Zilon.DependencyInjection;

namespace Zilon.Core.Specs.Contexts
{
    public static class IServiceCollectionExtensions
    {
        public static void RegisterClientServices(this ServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ISectorUiState, SectorUiState>();
            serviceCollection.AddScoped<IInventoryState, InventoryState>();
            serviceCollection.AddScoped<IAnimationBlockerService, AnimationBlockerService>();

            serviceCollection.AddScoped<ICommandLoopContext, CommandLoopContext>();
            serviceCollection.AddScoped<ICommandLoopUpdater, CommandLoopUpdater>();
            serviceCollection.AddScoped<ICommandPool, QueueCommandPool>();
        }

        public static void RegisterCommands(this ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<MoveCommand>();
            serviceCollection.AddSingleton<IdleCommand>();
            serviceCollection.AddSingleton<UseSelfCommand>();
            serviceCollection.AddSingleton<AttackCommand>();
            serviceCollection.AddSingleton<SectorTransitionMoveCommand>();

            serviceCollection.AddTransient<PropTransferCommand>();
            serviceCollection.AddTransient<EquipCommand>();
        }

        public static void RegisterGlobeInitializationServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IGlobeInitializer, GlobeInitializer>();
            serviceCollection.AddSingleton<IBiomeInitializer, BiomeInitializer>();
            serviceCollection.AddSingleton<IBiomeSchemeRoller, BiomeSchemeRoller>();
            serviceCollection.AddSingleton<IGlobeTransitionHandler, GlobeTransitionHandler>();
            serviceCollection.AddSingleton<IPersonInitializer, EmptyPersonInitializer>();
            serviceCollection.AddSingleton<ITransitionPool, TransitionPool>();
            serviceCollection.AddSingleton<IGlobeExpander>(serviceProvider =>
            {
                return (BiomeInitializer)serviceProvider.GetRequiredService<IBiomeInitializer>();
            });
        }

        public static void RegisterPlayerServices(this ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IPlayer, HumanPlayer>();
            serviceCollection
                .AddSingleton<IActorTaskSource<ISectorTaskSourceContext>,
                    HumanBotActorTaskSource<ISectorTaskSourceContext>>();
            serviceCollection
                .AddSingleton<IHumanActorTaskSource<ISectorTaskSourceContext>,
                    HumanActorTaskSource<ISectorTaskSourceContext>>();
            serviceCollection.AddSingleton<MonsterBotActorTaskSource<ISectorTaskSourceContext>>();
            serviceCollection.AddSingleton<IActorTaskSourceCollector>(serviceProvider =>
            {
                var humanTaskSource =
                    serviceProvider.GetRequiredService<IHumanActorTaskSource<ISectorTaskSourceContext>>();
                var monsterTaskSource =
                    serviceProvider.GetRequiredService<MonsterBotActorTaskSource<ISectorTaskSourceContext>>();
                return new ActorTaskSourceCollector(humanTaskSource, monsterTaskSource);
            });
            RegisterManager.RegisterBot(serviceCollection);
        }

        public static void RegisterSchemeService(this ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ISchemeLocator>(factory =>
            {
                var schemeLocator = FileSchemeLocator.CreateFromEnvVariable();

                return schemeLocator;
            });

            serviceCollection.AddSingleton<ISchemeService, SchemeService>();

            serviceCollection.AddSingleton<ISchemeServiceHandlerFactory, SchemeServiceHandlerFactory>();
        }

        public static void RegisterSectorService(this ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IMapFactory, FuncMapFactory>();
            serviceCollection.AddSingleton<ISectorGenerator, TestEmptySectorGenerator>();
            serviceCollection.AddSingleton<IRoomGenerator, RoomGenerator>();
            serviceCollection.AddSingleton<IScoreManager, ScoreManager>();
            serviceCollection.AddSingleton<IActorInteractionBus, ActorInteractionBus>();
            serviceCollection.AddSingleton<IPersonFactory, TestEmptyPersonFactory>();
            serviceCollection.AddSingleton<IMonsterPersonFactory, MonsterPersonFactory>();
        }

        public static void RegisterStaticObjectFactories(this IServiceCollection serviceCollection)
        {
            serviceCollection.RegisterStaticObjectFactoringFromCore();
        }
    }
}