using System;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Bot.Players.NetCore;
using Zilon.Bot.Players.NetCore.DependencyInjectionExtensions;
using Zilon.Bot.Players.Strategies;
using Zilon.Emulation.Common;

namespace Zilon.Bot.Players.DevelopmentTests
{
    class Startup: InitialzationBase
    {
        public override void ConfigureAux(IServiceProvider serviceFactory)
        {
            LogicStateTreePatterns.Factory = serviceFactory.GetRequiredService<ILogicStateFactory>();
        }

        protected override void RegisterBot(IServiceCollection container)
        {
            container.RegisterLogicState();
            container.AddScoped<ILogicStateFactory>(factory => new ContainerLogicStateFactory(factory));

            container.AddScoped<HumanBotActorTaskSource>();

        }
    }
}
