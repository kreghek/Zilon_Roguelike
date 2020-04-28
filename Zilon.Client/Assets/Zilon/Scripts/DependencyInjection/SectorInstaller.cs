using Assets.Zilon.Scripts.DependencyInjection;
using Assets.Zilon.Scripts.Services;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Client.Windows;
using Zilon.Core.Commands;
using Zilon.Core.Props;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.World;

public class SectorInstaller : MonoInstaller<SectorInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<ICommandManager>().To<QueueCommandManager>().AsSingle();

        Container.Bind<IGameLoop>().To<GameLoop>().AsSingle();
        Container.Bind<ISectorUiState>().To<SectorUiState>().AsSingle();

        Container.RegisterActorTaskSourcesServices();

        Container.RegisterActUsageService();

        Container.Bind<IEquipmentDurableService>().To<EquipmentDurableService>().AsSingle();
        Container.Bind<IEquipmentDurableServiceRandomSource>().To<EquipmentDurableServiceRandomSource>().AsSingle();

        Container.Bind<ISectorManager>().To<InfSectorManager>().AsSingle();
        Container.Bind<IBiomeInitializer>().To<BiomeInitializer>().AsSingle();
        Container.Bind<ISectorModalManager>().FromInstance(GetSectorModalManager()).AsSingle();
        Container.Bind<IActorInteractionBus>().To<ActorInteractionBus>().AsSingle();

        Container.Bind<IMineDepositMethodRandomSource>().To<MineDepositMethodRandomSource>().AsSingle();

        // генерация сектора
        Container.RegisterGenerationServices();

        // Специализированные сервисы для Ui.
        Container.Bind<IInventoryState>().To<InventoryState>().AsSingle();
        Container.Bind<ILogService>().To<LogService>().AsSingle();

        // Комманды актёра.
        Container.RegisterActorCommands();

        // Комадны для UI.
        Container.RegisterUiCommands();

        // Специализированные команды для Ui.
        Container.RegisterSpecialCommands();

        Container.RegisterStaticObjecServices();
    }

    private SectorModalManager GetSectorModalManager()
    {
        var sectorModalManager = FindObjectOfType<SectorModalManager>();
        return sectorModalManager;
    }
}