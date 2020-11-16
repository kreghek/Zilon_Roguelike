using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

namespace Zilon.Bot.Players.NetCore.DependencyInjectionExtensions
{
    public static class IServiceCollectionExtensions
    {
        public static void RegisterLogicState(this IServiceCollection serviceRegistry)
        {
            var logicTypes = GetTypes<ILogicState>();
            var triggerTypes = GetTypes<ILogicStateTrigger>()
                .Where(x => !typeof(ICompositLogicStateTrigger).IsAssignableFrom(x));

            var allTypes = logicTypes.Union(triggerTypes);
            foreach (var logicType in allTypes)
            {
                // Регистрируем, как трансиентные. Потому что нам может потребовать несколько
                // состояний и триггеров одного и того же типа.
                // Например, для различной кастомизации.
                serviceRegistry.AddTransient(logicType);
            }
        }

        private static IEnumerable<Type> GetTypes<TInterface>()
        {
            var logicTypes = typeof(ILogicState).Assembly.GetTypes()
                                                .Where(x => !x.IsAbstract && !x.IsInterface &&
                                                            typeof(TInterface).IsAssignableFrom(x));
            return logicTypes;
        }
    }
}