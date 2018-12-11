using Assets.Zilon.Scripts.Services;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.MapGenerators;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

public class SectorInstaller : MonoInstaller<SectorInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<IGameLoop>().To<GameLoop>().AsSingle();
        Container.Bind<ICommandManager>().To<QueueCommandManager>().AsSingle();
        Container.Bind<IPlayerState>().To<PlayerState>().AsSingle();
        Container.Bind<IActorManager>().To<ActorManager>().AsSingle();
        Container.Bind<IPropContainerManager>().To<PropContainerManager>().AsSingle();
        Container.Bind<IHumanActorTaskSource>().To<HumanActorTaskSource>().AsSingle();
        Container.Bind<IActorTaskSource>().WithId("monster").To<MonsterActorTaskSource>().AsSingle();
        Container.Bind<ISectorProceduralGenerator>().To<SectorProceduralGenerator>().AsSingle();
        Container.Bind<IMapFactory>().To<DungeonMapFactory>().AsSingle();
        Container.Bind<ITacticalActUsageService>().To<TacticalActUsageService>().AsSingle();
        Container.Bind<ITacticalActUsageRandomSource>().To<TacticalActUsageRandomSource>().AsSingle();
        Container.Bind<ISurvivalRandomSource>().To<SurvivalRandomSource>().AsSingle();

        Container.Bind<ISectorManager>().To<SectorManager>().AsSingle();
        Container.Bind<ISectorModalManager>().FromInstance(GetSectorModalManager()).AsSingle();


        // Специализированные сервисы для Ui.
        Container.Bind<IInventoryState>().To<InventoryState>().AsSingle();
        Container.Bind<ILogService>().To<LogService>().AsSingle();

        // Комманды актёра.
        Container.Bind<ICommand>().WithId("move-command").To<MoveCommand>().AsSingle();
        Container.Bind<ICommand>().WithId("attack-command").To<AttackCommand>().AsSingle();
        Container.Bind<ICommand>().WithId("open-container-command").To<OpenContainerCommand>().AsSingle();
        Container.Bind<ICommand>().WithId("next-turn-command").To<NextTurnCommand>().AsSingle();
        Container.Bind<ICommand>().WithId("use-self-command").To<UseSelfCommand>().AsSingle();

        // Комадны для UI.
        Container.Bind<ICommand>().WithId("show-container-modal-command").To<ShowContainerModalCommand>().AsSingle();
        Container.Bind<ICommand>().WithId("show-inventory-command").To<ShowInventoryModalCommand>().AsSingle();
        Container.Bind<ICommand>().WithId("show-perks-command").To<ShowPerksModalCommand>().AsSingle();

        // Специализированные команды для Ui.
        Container.Bind<ICommand>().WithId("equip-command").To<EquipCommand>().AsTransient();
        Container.Bind<ICommand>().WithId("prop-transfer-command").To<PropTransferCommand>().AsTransient();
    }


    private SectorModalManager GetSectorModalManager()
    {
        var sectorModalManager = FindObjectOfType<SectorModalManager>();
        Debug.Log(sectorModalManager);
        return sectorModalManager;
    }
}