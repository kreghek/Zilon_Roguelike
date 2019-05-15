using LightInject;

using Zilon.Bot.Players.Logics;
using Zilon.Bot.Players.Triggers;

namespace Zilon.Bot.Players.LightInject.DependencyInjection
{
    public static class LightInjectServiceContainerExtensions
    {
        public static void RegisterLogicState(this IServiceRegistry serviceRegistry)
        {
            //TODO Сделать автоматическую регистрацию всех тригеров и логик
            serviceRegistry.Register<DefeatTargetLogicState>();
            serviceRegistry.Register<IdleLogicState>();
            serviceRegistry.Register<RoamingLogicState>();
            serviceRegistry.Register<HealSelfLogicState>();
            serviceRegistry.Register<EatProviantLogicState>();

            serviceRegistry.Register<CounterOverTrigger>();
            serviceRegistry.Register<IntruderDetectedTrigger>();
            serviceRegistry.Register<LogicOverTrigger>();
            serviceRegistry.Register<LowHpAndHasResourceTrigger>();
            serviceRegistry.Register<HungryAndHasResourceTrigger>();
            serviceRegistry.Register<ThirstAndHasResourceTrigger>();
        }
    }
}
