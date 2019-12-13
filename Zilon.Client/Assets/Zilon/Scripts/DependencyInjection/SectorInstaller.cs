using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Zilon.Scripts.Commands;
using Assets.Zilon.Scripts.Services;

using Zenject;

using Zilon.Bot.Players;
using Zilon.Bot.Players.Strategies;
using Zilon.Core.Client;
using Zilon.Core.Client.Windows;
using Zilon.Core.Commands;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.CellularAutomatonStyle;
using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.Props;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.World;

public class SectorInstaller : MonoInstaller<SectorInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<ICommandManager<SectorCommandContext>>().To<QueueCommandManager<SectorCommandContext>>().AsSingle();
        Container.Bind<ICommandManager<ActorModalCommandContext>>().To<QueueCommandManager<ActorModalCommandContext>>().AsSingle();

        Container.Bind<ISectorUiState>().To<SectorUiState>().AsSingle();

        Container.Bind<IActorManager>().To<ActorManager>().AsTransient();
        Container.Bind<IPropContainerManager>().To<PropContainerManager>().AsTransient();

        Container.Bind<LogicStateTreePatterns>().AsSingle();
        Container.Bind<IHumanActorTaskSource>().To<HumanActorTaskSource>().AsSingle();
        Container.Bind<IActorTaskSource>().WithId("monster").To<MonsterBotActorTaskSource>().AsSingle();
        Container.Bind<ILogicStateFactory>().To<ZenjectLogicStateFactory>().AsSingle();
        RegisterBotLogics(Container);
        Container.Bind<ITacticalActUsageService>().To<TacticalActUsageService>().AsSingle()
            .OnInstantiated<TacticalActUsageService>((c, i) =>
             {
                 var equipmentDurableService = Container.Resolve<IEquipmentDurableService>();
                 if (equipmentDurableService != null)
                 {
                     i.EquipmentDurableService = equipmentDurableService;
                 }

                 var actorInteractionBus = Container.Resolve<IActorInteractionBus>();
                 if (actorInteractionBus != null)
                 {
                     i.ActorInteractionBus = actorInteractionBus;
                 }
             });
        Container.Bind<ITacticalActUsageRandomSource>().To<TacticalActUsageRandomSource>().AsSingle();
        Container.Bind<IEquipmentDurableService>().To<EquipmentDurableService>().AsSingle();
        Container.Bind<IEquipmentDurableServiceRandomSource>().To<EquipmentDurableServiceRandomSource>().AsSingle();

        Container.Bind<ISectorManager>().To<SectorManager>().AsSingle();
        Container.Bind<ISectorModalManager>().FromInstance(GetSectorModalManager()).AsSingle();
        Container.Bind<IActorInteractionBus>().To<ActorInteractionBus>().AsSingle();

        // генерация сектора
        Container.Bind<ISectorGenerator>().To<SectorGenerator>().AsSingle();
        Container.Bind<IMapFactory>().WithId("room").To<RoomMapFactory>().AsSingle();
        Container.Bind<IMapFactory>().WithId("cave").To<CellularAutomatonMapFactory>().AsSingle();
        Container.Bind<IMapFactorySelector>().To<MapFactorySelector>().AsSingle();
        Container.Bind<IRoomGeneratorRandomSource>().FromMethod(context => {
            var linearDice = context.Container.ResolveId<IDice>("linear");
            var roomSizeDice = context.Container.ResolveId<IDice>("exp");
            return new RoomGeneratorRandomSource(linearDice, roomSizeDice);
        }).AsSingle();
        Container.Bind<IInteriorObjectRandomSource>().To<InteriorObjectRandomSource>().AsSingle();
        Container.Bind<IRoomGenerator>().To<RoomGenerator>().AsSingle();
        Container.Bind<IChestGenerator>().To<ChestGenerator>().AsSingle();
        Container.Bind<IChestGeneratorRandomSource>().To<ChestGeneratorRandomSource>().AsSingle();
        Container.Bind<IMonsterGenerator>().To<MonsterGenerator>().AsSingle();
        Container.Bind<IMonsterGeneratorRandomSource>().To<MonsterGeneratorRandomSource>().AsSingle();
        Container.Bind<ISectorFactory>().To<SectorFactory>().AsSingle();
        Container.Bind<ICitizenGenerator>().To<CitizenGenerator>().AsSingle();
        Container.Bind<ICitizenGeneratorRandomSource>().To<CitizenGeneratorRandomSource>().AsSingle();


        // Специализированные сервисы для Ui.
        Container.Bind<IInventoryState>().To<InventoryState>().AsSingle();
        Container.Bind<ILogService>().To<LogService>().AsSingle();

        // Комманды актёра.
        Container.Bind<MoveCommand>().AsSingle();
        Container.Bind<ICommand<SectorCommandContext>>().WithId("move-command").FromMethod((context) =>
        {
            var globeManager = context.Container.Resolve<IGlobeManager>();
            var botTaskSource = context.Container.ResolveId<IActorTaskSource>("monster");
            var underlyingCommand = context.Container.Resolve<MoveCommand>();
            var commandWrapper = new UpdateGlobeCommand<SectorCommandContext>(
                globeManager,
                botTaskSource,
                underlyingCommand);
            return commandWrapper;
        }).AsSingle();
        Container.Bind<ICommand<SectorCommandContext>>().WithId("attack-command").To<AttackCommand>().AsSingle();
        Container.Bind<ICommand<SectorCommandContext>>().WithId("open-container-command").To<OpenContainerCommand>().AsSingle();
        Container.Bind<ICommand<SectorCommandContext>>().WithId("next-turn-command").To<NextTurnCommand>().AsSingle();
        Container.Bind<ICommand<SectorCommandContext>>().WithId("use-self-command").To<UseSelfCommand>().AsSingle();
        Container.Bind<ICommand<SectorCommandContext>>().WithId("sector-transition-move-command").To<SectorTransitionMoveCommand>().AsSingle();

        // Комадны для UI.
        Container.Bind<ICommand<ActorModalCommandContext>>().WithId("show-container-modal-command").To<ShowContainerModalCommand>().AsSingle();
        Container.Bind<ICommand<ActorModalCommandContext>>().WithId("show-inventory-command").To<ShowInventoryModalCommand>().AsSingle();
        Container.Bind<ICommand<ActorModalCommandContext>>().WithId("show-perks-command").To<ShowPerksModalCommand>().AsSingle();
        Container.Bind<ICommand<ActorModalCommandContext>>().WithId("show-trader-modal-command").To<ShowTraderModalCommand>().AsSingle();
        Container.Bind<ICommand<ActorModalCommandContext>>().WithId("show-dialog-modal-command").To<ShowDialogModalCommand>().AsSingle();
        Container.Bind<ICommand<ActorModalCommandContext>>().WithId("show-history-command").To<SectorShowHistoryCommand>().AsSingle();
        Container.Bind<ICommand<ActorModalCommandContext>>().WithId("quit-request-command").To<QuitRequestCommand>().AsSingle();
        Container.Bind<ICommand<ActorModalCommandContext>>().WithId("quit-request-title-command").To<QuitTitleRequestCommand>().AsSingle();

        // Специализированные команды для Ui.
        Container.Bind<ICommand<SectorCommandContext>>().WithId("equip-command").To<EquipCommand>().AsTransient();
        Container.Bind<ICommand<SectorCommandContext>>().WithId("prop-transfer-command").To<PropTransferCommand>().AsTransient();

        Container.Bind<SpecialCommandManager>().AsSingle();
    }

    private void RegisterBotLogics(DiContainer container)
    {
        var logicTypes = GetTypes<ILogicState>();
        var triggerTypes = GetTypes<ILogicStateTrigger>();

        var allTypes = logicTypes.Union(triggerTypes);
        foreach (var logicType in allTypes)
        {
            // Регистрируем, как трансиентные. Потому что нам может потребовать несколько
            // состояний и триггеров одного и того же типа.
            // Например, для различной кастомизации.

            container.Bind(logicType).AsTransient();
        }
    }

    private static IEnumerable<Type> GetTypes<TInterface>()
    {
        var logicTypes = typeof(ILogicState).Assembly.GetTypes()
            .Where(x => !x.IsAbstract && !x.IsInterface && typeof(TInterface).IsAssignableFrom(x));
        return logicTypes;
    }

    private SectorModalManager GetSectorModalManager()
    {
        var sectorModalManager = FindObjectOfType<SectorModalManager>();
        return sectorModalManager;
    }
}