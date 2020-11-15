using System;

using Zilon.Bot.Players.NetCore;
using Zilon.Emulation.Common;

namespace Zilon.GlobeObserver
{
    internal sealed class StartUp : InitializationBase
    {
        public override void ConfigureAux(IServiceProvider serviceFactory)
        {
            throw new NotImplementedException();
        }

        public override void RegisterServices(IServiceCollection serviceCollection)
        {
            base.RegisterServices(serviceCollection);

            serviceCollection.AddSingleton<IGlobeInitializer, GlobeInitializer>();
            serviceCollection.AddSingleton<IGlobeExpander>(provider =>
                (BiomeInitializer)provider.GetRequiredService<IBiomeInitializer>());
            serviceCollection.AddSingleton<IGlobeTransitionHandler, GlobeTransitionHandler>();
            serviceCollection.AddSingleton<IPersonInitializer, AutoPersonInitializer>();
        }

        protected override void RegisterBot(IServiceCollection serviceCollection)
        {
            serviceCollection.RegisterLogicState();
            serviceCollection.AddSingleton<ILogicStateFactory>(factory => new ContainerLogicStateFactory(factory));
            serviceCollection.AddSingleton<LogicStateTreePatterns>();

            serviceCollection
                .AddSingleton<IActorTaskSource<ISectorTaskSourceContext>,
                    HumanBotActorTaskSource<ISectorTaskSourceContext>>();
        }
    }
}