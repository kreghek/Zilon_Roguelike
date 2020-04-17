using Zenject;

using Zilon.Core.Props;
using Zilon.Core.Scoring;
using Zilon.Core.Tactics;

namespace Assets.Zilon.Scripts.DependencyInjection
{
    public static class TacticalActUsageServiceExtensions
    {
        public static void RegisterTacticalActUsageService(this DiContainer diContainer)
        {
            diContainer.Bind<ITacticalActUsageService>().To<TacticalActUsageService>().AsSingle()
            .OnInstantiated<TacticalActUsageService>((c, service) =>
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

            diContainer.Bind<ITacticalActUsageRandomSource>().To<TacticalActUsageRandomSource>().AsSingle();
        }
    }
}
