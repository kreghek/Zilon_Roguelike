using LightInject;

using Zilon.Bot.Players.Logics;
using Zilon.Bot.Players.Triggers;

namespace Zilon.Bot.Players.DependencyInjection
{
    public static class LightInjectServiceContainerExtensions
    {
        public static void RegisterBot(this IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<DefeatTargetLogicState>(new PerContainerLifetime());
            serviceRegistry.Register<IdleLogicState>(new PerContainerLifetime());
            serviceRegistry.Register<RoamingLogicState>(new PerContainerLifetime());

            serviceRegistry.Register<CounterOverTrigger>(new PerContainerLifetime());
            serviceRegistry.Register<IntruderDetectedTrigger>(new PerContainerLifetime());
            serviceRegistry.Register<LogicOverTrigger>(new PerContainerLifetime());


        }
    }
}
