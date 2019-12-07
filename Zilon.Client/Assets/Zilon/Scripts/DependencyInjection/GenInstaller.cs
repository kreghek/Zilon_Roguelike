using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Zilon.Scripts.Services;

using Zenject;

using Zilon.Bot.Players;
using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.Props;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

public class GenInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        RegisterRoomMapFactory();

        Container.Bind<ICommandManager>().To<QueueCommandManager>().AsSingle();

        Container.Bind<ISectorUiState>().To<SectorUiState>().AsSingle();

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

        Container.Bind<IActorInteractionBus>().To<ActorInteractionBus>().AsSingle();

        // —пециализированные сервисы дл€ Ui.
        Container.Bind<IInventoryState>().To<InventoryState>().AsSingle();
        Container.Bind<ILogService>().To<LogService>().AsSingle();
    }

    private void RegisterBotLogics(DiContainer container)
    {
        var logicTypes = GetTypes<ILogicState>();
        var triggerTypes = GetTypes<ILogicStateTrigger>();

        var allTypes = logicTypes.Union(triggerTypes);
        foreach (var logicType in allTypes)
        {
            // –егистрируем, как трансиентные. ѕотому что нам может потребовать несколько
            // состо€ний и триггеров одного и того же типа.
            // Ќапример, дл€ различной кастомизации.

            container.Bind(logicType).AsTransient();
        }
    }

    private static IEnumerable<Type> GetTypes<TInterface>()
    {
        var logicTypes = typeof(ILogicState).Assembly.GetTypes()
            .Where(x => !x.IsAbstract && !x.IsInterface && typeof(TInterface).IsAssignableFrom(x));
        return logicTypes;
    }

    private void RegisterRoomMapFactory()
    {
        Container.Bind<IMapFactory>().To<RoomMapFactory>().AsSingle();
        Container.Bind<IRoomGenerator>().To<RoomGenerator>().AsSingle();
        Container.Bind<IRoomGeneratorRandomSource>().FromMethod(context => {
            var linearDice = context.Container.ResolveId<IDice>("linear");
            var roomSizeDice = context.Container.ResolveId<IDice>("exp");
            return new RoomGeneratorRandomSource(linearDice, roomSizeDice);
        }).AsSingle();
    }
}