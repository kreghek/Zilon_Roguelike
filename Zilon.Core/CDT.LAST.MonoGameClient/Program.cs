﻿using System;
using System.Globalization;
using System.Threading;

using CDT.LAST.MonoGameClient.Database;
using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.GameComponents;
using CDT.LAST.MonoGameClient.Screens;
using CDT.LAST.MonoGameClient.ViewModels.MainScene;
using CDT.LAST.MonoGameClient.ViewModels.MainScene.GameObjectVisualization;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.Commands;
using Zilon.Core.PersonGeneration;
using Zilon.Core.Players;
using Zilon.Core.ScoreResultGenerating;
using Zilon.Core.Tactics;
using Zilon.Core.World;

namespace CDT.LAST.MonoGameClient
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            var defaultCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = defaultCulture;
            Thread.CurrentThread.CurrentUICulture = defaultCulture;

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
            serviceContainer.AddSingleton<DbContext>();

            RegisterCommands(serviceContainer);
            RegisterUiContentStorages(serviceContainer);

            serviceContainer.AddSingleton<IPersonVisualizationContentStorage, PersonVisualizationContentStorage>();
            serviceContainer.AddSingleton<IPersonSoundContentStorage, PersonSoundContentStorage>();
            serviceContainer
                .AddSingleton<IGameObjectVisualizationContentStorage, GameObjectVisualizationContentStorage>();
            serviceContainer.AddSingleton<SoundtrackManager>();

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
            serviceContainer.AddScoped<UseSelfCommand>();
            serviceContainer.AddScoped<OpenContainerCommand>();

            serviceContainer.AddTransient<EquipCommand>();
            serviceContainer.AddTransient<PropTransferCommand>();
        }

        private static void RegisterUiContentStorages(ServiceCollection serviceContainer)
        {
            serviceContainer.AddSingleton<IUiContentStorage, UiContentStorage>();
            serviceContainer.AddSingleton<IUiSoundStorage, UiSoundStorage>();
        }
    }
}