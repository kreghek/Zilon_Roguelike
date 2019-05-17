using System.Linq;
using LightInject;

using Zilon.Bot.Players.Logics;
using Zilon.Bot.Players.Triggers;

namespace Zilon.Bot.Players.LightInject.DependencyInjection
{
    public static class LightInjectServiceContainerExtensions
    {
        public static void RegisterLogicState(this IServiceRegistry serviceRegistry)
        {
            var logicTypes = typeof(ILogicState).Assembly.GetTypes()
                .Where(x => !x.IsAbstract && !x.IsInterface && typeof(ILogicState).IsAssignableFrom(x));
            foreach (var logicType in logicTypes)
            {
                serviceRegistry.Register(logicType);
            }

            var triggerTypes = typeof(ILogicStateTrigger).Assembly.GetTypes()
                .Where(x => !x.IsAbstract && !x.IsInterface && typeof(ILogicStateTrigger).IsAssignableFrom(x));
            foreach (var triggerType in triggerTypes)
            {
                serviceRegistry.Register(triggerType);
            }
        }
    }
}
