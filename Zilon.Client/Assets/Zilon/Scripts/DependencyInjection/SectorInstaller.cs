﻿using Assets.Zilon.Scripts.DependencyInjection;
using Assets.Zilon.Scripts.Services;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Client.Sector;
using Zilon.Core.Client.Windows;
using Zilon.Core.Commands;
using Zilon.Core.Props;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.World;

public class SectorInstaller : MonoInstaller<SectorInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<IGlobeLoopUpdater>().To<GlobeLoopUpdater>().AsSingle();
        Container.Bind<IGlobeLoopContext>().To<GlobeLoopContext>().AsSingle();
        Container.Bind<ICommandLoopUpdater>().To<CommandLoopUpdater>().AsSingle();
        Container.Bind<ICommandLoopContext>().To<CommandLoopContext>().AsSingle();

        Container.Bind<ICommandPool>().To<QueueCommandPool>().AsSingle();

        Container.Bind<ISectorUiState>().To<SectorUiState>().AsSingle();

        Container.Bind<IEquipmentDurableService>().To<EquipmentDurableService>().AsSingle();
        Container.Bind<IEquipmentDurableServiceRandomSource>().To<EquipmentDurableServiceRandomSource>().AsSingle();

        Container.Bind<IBiomeInitializer>().To<BiomeInitializer>().AsSingle();
        Container.Bind<ISectorModalManager>().FromInstance(GetSectorModalManager()).AsSingle();

        Container.Bind<IMineDepositMethodRandomSource>().To<MineDepositMethodRandomSource>().AsSingle();

        // Специализированные сервисы для Ui.
        Container.Bind<IInventoryState>().To<InventoryState>().AsSingle();
        Container.Bind<ILogService>().To<LogService>().AsSingle();

        // Комманды актёра.
        Container.RegisterActorCommands();

        // Комадны для UI.
        Container.RegisterUiCommands();

        // Специализированные команды для Ui.
        Container.RegisterSpecialCommands();
    }

    private SectorModalManager GetSectorModalManager()
    {
        var sectorModalManager = FindObjectOfType<SectorModalManager>();
        return sectorModalManager;
    }
}