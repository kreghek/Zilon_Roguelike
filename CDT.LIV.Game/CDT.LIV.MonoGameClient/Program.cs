using System;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Bot.Players;
using Zilon.Bot.Players.NetCore;
using Zilon.Bot.Players.NetCore.DependencyInjectionExtensions;
using Zilon.Bot.Players.Strategies;
using Zilon.Core.Client.Sector;
using Zilon.Core.Commands;
using Zilon.Core.PersonGeneration;
using Zilon.Core.Players;
using Zilon.Core.World;
using Zilon.Emulation.Common;

using HumanActorTaskSource =
    Zilon.Core.Tactics.Behaviour.HumanActorTaskSource<Zilon.Core.Tactics.Behaviour.ISectorTaskSourceContext>;
using IActorTaskSource =
    Zilon.Core.Tactics.Behaviour.IActorTaskSource<Zilon.Core.Tactics.Behaviour.ISectorTaskSourceContext>;

namespace CDT.LIV.MonoGameClient
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            var serviceContainer = new ServiceCollection();
            var startUp = new StartUp();
            startUp.RegisterServices(serviceContainer);

            serviceContainer.AddSingleton<IGlobeInitializer, GlobeInitializer>();
            serviceContainer.AddSingleton<IGlobeExpander>(provider =>
                (BiomeInitializer)provider.GetRequiredService<IBiomeInitializer>());
            serviceContainer.AddSingleton<IGlobeTransitionHandler, GlobeTransitionHandler>();
            serviceContainer.AddSingleton<IPersonInitializer, HumanPersonInitializer>();
            serviceContainer.AddSingleton<IPlayer, HumanPlayer>();
            serviceContainer.AddScoped<MoveCommand>();
            serviceContainer.AddScoped<IdleCommand>();
            serviceContainer.AddScoped<AttackCommand>();
            serviceContainer.AddSingleton<IMonsterIdentifierGenerator, MonsterIdentifierGenerator>();
            serviceContainer.AddScoped<SectorTransitionMoveCommand>();
            serviceContainer.AddScoped<ICommandPool, QueueCommandPool>();
            serviceContainer.AddScoped<IAnimationBlockerService, AnimationBlockerService>();

            serviceContainer.AddSingleton<IGlobeLoopUpdater, GlobeLoopUpdater>();
            serviceContainer.AddSingleton<IGlobeLoopContext, GlobeLoopContext>();

            using var serviceProvider = serviceContainer.BuildServiceProvider();


            using var game = new LivGame(serviceProvider);

            game.Run();
        }

        internal sealed class StartUp : InitializationBase
        {
            public override void ConfigureAux(IServiceProvider serviceFactory)
            {
                throw new NotImplementedException();
            }

            protected override void RegisterBot(IServiceCollection serviceCollection)
            {
                serviceCollection.RegisterLogicState();
                serviceCollection.AddSingleton<ILogicStateFactory>(factory => new ContainerLogicStateFactory(factory));
                serviceCollection.AddSingleton<LogicStateTreePatterns>();

                serviceCollection.AddSingleton<IActorTaskSource, HumanActorTaskSource>();
            }
        }
    }
}
