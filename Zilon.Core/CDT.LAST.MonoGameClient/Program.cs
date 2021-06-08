using System;
using System.Globalization;
using System.Threading;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Screens;
using CDT.LAST.MonoGameClient.ViewModels.MainScene;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.Commands;
using Zilon.Core.PersonGeneration;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.World;

namespace CDT.LAST.MonoGameClient
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            var newCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newCulture;

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
            serviceContainer.AddSingleton<IActorInteractionBus, ActorInteractionBus>();

            RegisterCommands(serviceContainer);
            RegisterUiContentStorages(serviceContainer);

            serviceContainer.AddSingleton<IPersonVisualizationContentStorage, PersonVisualizationContentStorage>();
            serviceContainer.AddSingleton<IPersonSoundContentStorage, PersonSoundContentStorage>();

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
            serviceContainer.AddTransient<EquipCommand>();
            serviceContainer.AddScoped<UseSelfCommand>();
            serviceContainer.AddScoped<OpenContainerCommand>();
        }

        private static void RegisterUiContentStorages(ServiceCollection serviceContainer)
        {
            serviceContainer.AddSingleton<IUiContentStorage, UiContentStorage>();
            serviceContainer.AddSingleton<IUiSoundStorage, UiSoundStorage>();
        }
    }
}