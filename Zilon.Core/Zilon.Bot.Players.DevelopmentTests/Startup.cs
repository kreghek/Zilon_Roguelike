using System;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Bot.Players.NetCore;
using Zilon.Bot.Players.NetCore.DependencyInjectionExtensions;
using Zilon.Bot.Players.Strategies;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Emulation.Common;

namespace Zilon.Bot.Players.DevelopmentTests
{
    class Startup : InitializationBase
    {
        public override void ConfigureAux(IServiceProvider serviceProvider)
        {
            // Конфигурация дополнительных сервисов для коробочного источника команд не требуется.
        }

        protected override void RegisterBot(IServiceCollection container)
        {
            container.RegisterLogicState();
            container.AddSingleton<ILogicStateFactory>(factory => new ContainerLogicStateFactory(factory));
            container.AddSingleton<LogicStateTreePatterns>();

            container.AddSingleton<HumanBotActorTaskSource<ISectorTaskSourceContext>>();
            //container.AddSingleton<IActorTaskSource<ISectorTaskSourceContext>>(factory => factory.GetRequiredService<HumanBotActorTaskSource<ISectorTaskSourceContext>>());
        }
    }
}
