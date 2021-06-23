using System;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Bot.Players;
using Zilon.Bot.Players.NetCore;
using Zilon.Bot.Players.NetCore.DependencyInjectionExtensions;
using Zilon.Bot.Players.Strategies;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Emulation.Common;

using HumanActorTaskSource =
    Zilon.Core.Tactics.Behaviour.HumanActorTaskSource<Zilon.Core.Tactics.Behaviour.ISectorTaskSourceContext>;
using IActorTaskSource =
    Zilon.Core.Tactics.Behaviour.IActorTaskSource<Zilon.Core.Tactics.Behaviour.ISectorTaskSourceContext>;
using IHumanActorTaskSource =
    Zilon.Core.Tactics.Behaviour.IHumanActorTaskSource<Zilon.Core.Tactics.Behaviour.ISectorTaskSourceContext>;


namespace Zilon.Core.Benchmarks.CreateSector
{
    internal class Startup : InitializationBase
    {
        public Startup() : base(123)
        {
        }

        public override void ConfigureAux(IServiceProvider serviceFactory)
        {
            // Сейчас конфигурирование доп.сервисов для базового бота не требуется.
        }

        protected override void RegisterBot(IServiceCollection serviceCollection)
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
    }
}