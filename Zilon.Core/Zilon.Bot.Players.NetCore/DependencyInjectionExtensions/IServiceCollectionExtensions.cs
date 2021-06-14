using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Bot.Players.Strategies;
using Zilon.Core.Tactics.Behaviour;
using Zilon.DependencyInjection;

using HumanActorTaskSource =
    Zilon.Core.Tactics.Behaviour.HumanActorTaskSource<Zilon.Core.Tactics.Behaviour.ISectorTaskSourceContext>;
using IActorTaskSource =
    Zilon.Core.Tactics.Behaviour.IActorTaskSource<Zilon.Core.Tactics.Behaviour.ISectorTaskSourceContext>;
using IHumanActorTaskSource =
    Zilon.Core.Tactics.Behaviour.IHumanActorTaskSource<Zilon.Core.Tactics.Behaviour.ISectorTaskSourceContext>;

namespace Zilon.Bot.Players.NetCore.DependencyInjectionExtensions
{
    public static class IServiceCollectionExtensions
    {
        public static void RegisterBot(this IServiceCollection serviceCollection)
        {
            serviceCollection.RegisterLogicState();
            serviceCollection.AddSingleton<ILogicStateFactory>(factory => new ContainerLogicStateFactory(factory));
            serviceCollection.AddSingleton<LogicStateTreePatterns>();

            serviceCollection.AddSingleton<IHumanActorTaskSource>(serviceProvider =>
            {
                var humanTaskSource = new HumanActorTaskSource();
                var treePatterns = serviceProvider.GetRequiredService<LogicStateTreePatterns>();
                var botTaskSource = new HumanBotActorTaskSource<ISectorTaskSourceContext>(treePatterns);

                var switchTaskSource =
                    new SwitchHumanActorTaskSource<ISectorTaskSourceContext>(humanTaskSource, botTaskSource);
                return switchTaskSource;
            });
            serviceCollection.AddSingleton<IActorTaskSource>(provider =>
                provider.GetRequiredService<IHumanActorTaskSource>());
        }

        public static void RegisterLogicState(this IServiceCollection serviceRegistry)
        {
            var logicTypes = GetTypes<ILogicState>();
            var triggerTypes = GetTypes<ILogicStateTrigger>();

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