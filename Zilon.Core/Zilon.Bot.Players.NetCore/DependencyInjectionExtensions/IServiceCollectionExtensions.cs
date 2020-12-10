using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using Zilon.DependencyInjection;

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
            var assembly = typeof(ILogicState).Assembly;

            var logicTypes = ImplementationGatheringHelper.GetImplementations<TInterface>(assembly);

            return logicTypes;
        }
    }
}