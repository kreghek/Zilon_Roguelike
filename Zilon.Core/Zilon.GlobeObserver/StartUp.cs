using System;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Bot.Players;
using Zilon.Bot.Players.NetCore;
using Zilon.Bot.Players.NetCore.DependencyInjectionExtensions;
using Zilon.Bot.Players.Strategies;
using Zilon.Emulation.Common;

namespace Zilon.GlobeObserver
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
            serviceCollection.AddScoped<ILogicStateFactory>(factory => new ContainerLogicStateFactory(factory));
            serviceCollection.AddScoped<LogicStateTreePatterns>();

            serviceCollection.AddScoped<HumanBotActorTaskSource>();
        }
    }
}
