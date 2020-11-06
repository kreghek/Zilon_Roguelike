using Zenject;

using Zilon.Core.Props;
using Zilon.Core.Scoring;
using Zilon.Core.Tactics;

namespace Assets.Zilon.Scripts.DependencyInjection
{
    public static class TacticalActUsageServiceExtensions
    {
        public static void RegisterActUsageService(this DiContainer diContainer)
        {
            diContainer.Bind<ITacticalActUsageService>().To<TacticalActUsageService>().AsSingle()
            .OnInstantiated<TacticalActUsageService>((c, service) =>
            {
                var equipmentDurableService = diContainer.Resolve<IEquipmentDurableService>();
                service.EquipmentDurableService = equipmentDurableService;
            });

            diContainer.Bind<ITacticalActUsageRandomSource>().To<TacticalActUsageRandomSource>().AsSingle();

            diContainer.Bind<IActUsageHandlerSelector>().FromMethod(() =>
            {
                var handlers = diContainer.ResolveAll<IActUsageHandler>().ToArray();
                var service = new ActUsageHandlerSelector(handlers);
                return service;
            }).AsSingle();

            diContainer.Bind<IActUsageHandler>().To<ActorActUsageHandler>().AsSingle()
                .OnInstantiated<ActorActUsageHandler>((contex, service) =>
                {
                    var equipmentDurableService = diContainer.Resolve<IEquipmentDurableService>();
                    service.EquipmentDurableService = equipmentDurableService;

                    var actorInteractionBus = diContainer.Resolve<IActorInteractionBus>();
                    service.ActorInteractionBus = actorInteractionBus;

                    var playerEventLogService = diContainer.Resolve<IPlayerEventLogService>();
                    service.PlayerEventLogService = playerEventLogService;

                    var scoreManager = diContainer.Resolve<IScoreManager>();
                    service.ScoreManager = scoreManager;
                });

            diContainer.Bind<IActUsageHandler>().To<StaticObjectActUsageHandler>().AsSingle();
        }
    }
}
