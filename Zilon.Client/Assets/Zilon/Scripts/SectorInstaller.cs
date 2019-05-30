using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Zilon.Scripts.Commands;
using Assets.Zilon.Scripts.Services;

using Zenject;

using Zilon.Bot.Players;
using Zilon.Bot.Players.Strategies;
using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

public class SectorInstaller : MonoInstaller<SectorInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<ICommandManager>().To<QueueCommandManager>().AsSingle();

        Container.Bind<IGameLoop>().To<GameLoop>().AsSingle();
        Container.Bind<ISectorUiState>().To<SectorUiState>().AsSingle();
        Container.Bind<IActorManager>().To<ActorManager>().AsSingle();
        Container.Bind<IPropContainerManager>().To<PropContainerManager>().AsSingle();
        Container.Bind<ITraderManager>().To<TraderManager>().AsSingle();
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

             });
        Container.Bind<ITacticalActUsageRandomSource>().To<TacticalActUsageRandomSource>().AsSingle();
        Container.Bind<IEquipmentDurableService>().To<EquipmentDurableService>().AsSingle();
        Container.Bind<IEquipmentDurableServiceRandomSource>().To<EquipmentDurableServiceRandomSource>().AsSingle();

        Container.Bind<ISectorManager>().To<SectorManager>().AsSingle();
        Container.Bind<ISectorModalManager>().FromInstance(GetSectorModalManager()).AsSingle();

        // генерация сектора
        Container.Bind<ISectorGenerator>().To<SectorGenerator>().AsSingle();
        Container.Bind<IMapFactory>().To<RoomMapFactory>().AsSingle();
        Container.Bind<IRoomGeneratorRandomSource>().To<RoomGeneratorRandomSource>().AsSingle();
        Container.Bind<IRoomGenerator>().To<RoomGenerator>().AsSingle();
        Container.Bind<IChestGenerator>().To<ChestGenerator>().AsSingle();
        Container.Bind<IChestGeneratorRandomSource>().To<ChestGeneratorRandomSource>().AsSingle();
        Container.Bind<IMonsterGenerator>().To<MonsterGenerator>().AsSingle();
        Container.Bind<IMonsterGeneratorRandomSource>().To<MonsterGeneratorRandomSource>().AsSingle();
        Container.Bind<ISectorFactory>().To<SectorFactory>().AsSingle();


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
        Container.Bind<ICommand>().WithId("show-trader-modal-command").To<ShowTraderModalCommand>().AsSingle();

        // Специализированные команды для Ui.
        Container.Bind<ICommand>().WithId("equip-command").To<EquipCommand>().AsTransient();
        Container.Bind<ICommand>().WithId("prop-transfer-command").To<PropTransferCommand>().AsTransient();
        Container.Bind<ICommand>().WithId("quit-request-command").To<QuitRequestCommand>().AsSingle();
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