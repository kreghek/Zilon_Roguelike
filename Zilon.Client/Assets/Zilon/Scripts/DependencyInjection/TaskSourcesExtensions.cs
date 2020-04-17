using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Zilon.Scripts.Models.Globe;
using Assets.Zilon.Scripts.Services;

using Zenject;

using Zilon.Bot.Players;
using Zilon.Bot.Players.Strategies;
using Zilon.Core.Commands;
using Zilon.Core.Tactics.Behaviour;

namespace Assets.Zilon.Scripts.DependencyInjection
{
    public static class TaskSourcesExtensions
    {
        public static void RegisterActorTaskSourcesServices(this DiContainer diContainer)
        {
            diContainer.Bind<IHumanActorTaskSource>().To<HumanActorTaskSource>().AsSingle();
            diContainer.Bind<IActorTaskSource>().WithId("monster").To<MonsterBotActorTaskSource>().AsSingle();
            diContainer.Bind<IActorTaskSourceCollector>().FromMethod(context =>
            {
                var botTaskSource = context.Container.ResolveId<IActorTaskSource>("monster");
                var humanTaskSource = context.Container.Resolve<IHumanActorTaskSource>();
                return new TaskSourceCollector(botTaskSource, humanTaskSource);
            }).AsSingle();
            diContainer.Bind<LogicStateTreePatterns>().AsSingle();
            diContainer.Bind<ILogicStateFactory>().To<ZenjectLogicStateFactory>().AsSingle();
            RegisterBotLogics(diContainer);
        }

        private static void RegisterBotLogics(DiContainer diContainer)
        {
            var logicTypes = GetTypes<ILogicState>();
            var triggerTypes = GetTypes<ILogicStateTrigger>();

            var allTypes = logicTypes.Union(triggerTypes);
            foreach (var logicType in allTypes)
            {
                // Регистрируем, как трансиентные. Потому что нам может потребовать несколько
                // состояний и триггеров одного и того же типа.
                // Например, для различной кастомизации.

                diContainer.Bind(logicType).AsTransient();
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
