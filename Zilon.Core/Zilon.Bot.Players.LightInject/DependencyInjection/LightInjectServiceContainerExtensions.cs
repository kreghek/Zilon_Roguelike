using System;
using System.Collections.Generic;
using System.Linq;

using LightInject;

namespace Zilon.Bot.Players.LightInject.DependencyInjection
{
    public static class LightInjectServiceContainerExtensions
    {
        public static void RegisterLogicState(this IServiceRegistry serviceRegistry)
        {
            var logicTypes = GetTypes<ILogicState>();
            var triggerTypes = GetTypes<ILogicStateTrigger>();

            var allTypes = logicTypes.Union(triggerTypes);
            foreach (var logicType in allTypes)
            {
                // Регистрируем, как персистентные. Потому что нам может потребовать несколько
                // состояний и триггеров одного и того же типа.
                // Например, для различной кастомизации.
                serviceRegistry.Register(logicType);
            }
        }

        private static IEnumerable<Type> GetTypes<TInterface>()
        {
            var logicTypes = typeof(ILogicState).Assembly.GetTypes()
                .Where(x => !x.IsAbstract && !x.IsInterface && typeof(TInterface).IsAssignableFrom(x));
            return logicTypes;
        }
    }
}
