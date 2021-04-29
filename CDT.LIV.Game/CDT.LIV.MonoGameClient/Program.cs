using System;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.Client.Sector;
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
            serviceContainer.AddScoped<MoveCommand>();
            serviceContainer.AddScoped<IdleCommand>();
            serviceContainer.AddScoped<AttackCommand>();
            serviceContainer.AddSingleton<IMonsterIdentifierGenerator, MonsterIdentifierGenerator>();
            serviceContainer.AddScoped<SectorTransitionMoveCommand>();

            using var serviceProvider = serviceContainer.BuildServiceProvider();


            using var game = new LivGame(serviceProvider);

            game.Run();
        }
    }
}
