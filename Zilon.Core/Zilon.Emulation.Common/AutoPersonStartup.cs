﻿using System;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Bot.Players;
using Zilon.Bot.Players.NetCore;
using Zilon.Bot.Players.NetCore.DependencyInjectionExtensions;
using Zilon.Bot.Players.Strategies;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.World;

namespace Zilon.Emulation.Common
{
    public class AutoPersonStartup : InitializationBase
    {
        public AutoPersonStartup() : base(1)
        {
        }

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