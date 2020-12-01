using System;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Bot.Players;
using Zilon.Bot.Players.NetCore;
using Zilon.Bot.Players.NetCore.DependencyInjectionExtensions;
using Zilon.Bot.Players.Strategies;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Emulation.Common;

namespace Zilon.Core.MassSectorGenerator
{
    public class Startup : InitializationBase
    {
        public Startup(int diceSeed) : base(diceSeed)
        {
        }

        public override void ConfigureAux(IServiceProvider serviceFactory)
        {
            // Для этой утилиты бота не настраиваем. Нам нужны только сектора.
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