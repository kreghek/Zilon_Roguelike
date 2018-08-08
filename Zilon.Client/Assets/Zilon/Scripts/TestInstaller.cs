using Assets.Zilon.Scripts.Models.SectorScene;
using Assets.Zilon.Scripts.Services;
using UnityEngine;
using Zenject;
using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.CommonServices.MapGenerators;
using Zilon.Core.MapGenerators;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour.Bots;

public class TestInstaller : MonoInstaller<TestInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<ICommandManager>().To<QueueCommandManager>().AsSingle();
        Container.Bind<ISectorManager>().To<SectorManager>().AsSingle();
        Container.Bind<IMapGenerator>().To<GridMapGenerator>().AsSingle();
        Container.Bind<IPlayerState>().To<PlayerState>().AsSingle();
        Container.Bind<IDice>().FromInstance(new Dice()).AsSingle(); // инстанцируем явно из-за 2-х конструкторов.
        Container.Bind<IDecisionSource>().To<DecisionSource>().AsSingle();
        Container.Bind<ISectorGeneratorRandomSource>().To<SectorGeneratorRandomSource>().AsSingle();
        Container.Bind<ISchemeService>().To<SchemeService>().AsSingle();
        Container.Bind<IPropFactory>().To<PropFactory>().AsSingle();
        Container.Bind<IDropResolver>().To<DropResolver>().AsSingle();
        Container.Bind<IDropResolverRandomSource>().To<DropResolverRandomSource>().AsSingle();
        Container.Bind<IPerkResolver>().To<PerkResolver>().AsSingle();
        Container.Bind<ITacticalActUsageService>().To<TacticalActUsageService>().AsSingle();
        Container.Bind<IActorManager>().To<ActorManager>().AsSingle();
        Container.Bind<IPropContainerManager>().To<PropContainerManager>().AsSingle();
        Container.Bind<ISchemeServiceHandlerFactory>().To<SchemeServiceHandlerFactory>().AsSingle();

        Container.Bind<HumanPlayer>().AsSingle();
        Container.Bind<BotPlayer>().AsSingle();

        Container.Bind<ISchemeLocator>().FromInstance(GetSchemeLocator()).AsSingle();
        Container.Bind<ISectorModalManager>().FromInstance(GetSectorModalManager()).AsSingle();
        
        // Специализированные сервисы для Ui.
        Container.Bind<IInventoryState>().To<InventoryState>().AsSingle();

        // Комманды актёра.
        Container.Bind<ICommand>().WithId("move-command").To<MoveCommand>().AsSingle();
        Container.Bind<ICommand>().WithId("attack-command").To<AttackCommand>().AsSingle();
        Container.Bind<ICommand>().WithId("open-container-command").To<OpenContainerCommand>().AsSingle();
        Container.Bind<ICommand>().WithId("next-turn-command").To<NextTurnCommand>().AsSingle();
        
        // Комадны для UI.
        Container.Bind<ICommand>().WithId("show-container-modal-command").To<ShowContainerModalCommand>().AsSingle();
        Container.Bind<ICommand>().WithId("show-inventory-command").To<ShowInventoryModalCommand>().AsSingle();
        
        // Специализированные команды для Ui.
        Container.Bind<ICommand>().WithId("equip-command").To<EquipCommand>().AsTransient();
    }

    private SchemeLocator GetSchemeLocator()
    {
        var schemeLocator = FindObjectOfType<SchemeLocator>();
        Debug.Log(schemeLocator);
        return schemeLocator;
    }

    private SectorModalManager GetSectorModalManager()
    {
        var sectorModalManager = FindObjectOfType<SectorModalManager>();
        Debug.Log(sectorModalManager);
        return sectorModalManager;
    }
}