using System;
using Zilon.Bot.Players.NetCore;
using Zilon.Emulation.Common;

namespace Zilon.TextClient
{
    internal sealed class StartUp : InitializationBase
    {
        public override void ConfigureAux(IServiceProvider serviceFactory)
        {
            throw new NotImplementedException();
        }

        protected override void RegisterBot(IServiceCollection serviceCollection)
        {
            serviceCollection.RegisterLogicState();
            serviceCollection.AddSingleton<ILogicStateFactory>(factory => new ContainerLogicStateFactory(factory));
            serviceCollection.AddSingleton<LogicStateTreePatterns>();

            serviceCollection
                .AddSingleton<IActorTaskSource<ISectorTaskSourceContext>, HumanActorTaskSource<ISectorTaskSourceContext>
                >();
        }
    }
}