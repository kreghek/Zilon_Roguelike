using System;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.Commands;
using Zilon.Core.PersonGeneration;
using Zilon.Core.Players;
using Zilon.Core.World;

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
            serviceContainer.AddSingleton<IMonsterIdentifierGenerator, MonsterIdentifierGenerator>();

            RegisterCommands(serviceContainer);

            using var serviceProvider = serviceContainer.BuildServiceProvider();


            using var game = new LivGame(serviceProvider);

            game.Run();
        }

        private static void RegisterCommands(ServiceCollection serviceContainer)
        {
            serviceContainer.AddScoped<MoveCommand>();
            serviceContainer.AddScoped<IdleCommand>();
            serviceContainer.AddScoped<AttackCommand>();
            serviceContainer.AddScoped<SectorTransitionMoveCommand>();
        }
    }
}
