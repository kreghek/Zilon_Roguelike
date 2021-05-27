using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Bot.Players;
using Zilon.Bot.Players.NetCore;
using Zilon.Bot.Players.NetCore.DependencyInjectionExtensions;
using Zilon.Bot.Players.Strategies;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Emulation.Common;

using HumanActorTaskSource =
    Zilon.Core.Tactics.Behaviour.HumanActorTaskSource<Zilon.Core.Tactics.Behaviour.ISectorTaskSourceContext>;
using IActorTaskSource =
    Zilon.Core.Tactics.Behaviour.IActorTaskSource<Zilon.Core.Tactics.Behaviour.ISectorTaskSourceContext>;
using IHumanActorTaskSource =
    Zilon.Core.Tactics.Behaviour.IHumanActorTaskSource<Zilon.Core.Tactics.Behaviour.ISectorTaskSourceContext>;

namespace CDT.LAST.MonoGameClient
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

        protected override void RegisterSchemeService(IServiceCollection container)
        {
            container.AddSingleton((Func<IServiceProvider, ISchemeLocator>)(factory =>
            {
                var binPath = AppContext.BaseDirectory;

                if (string.IsNullOrWhiteSpace(binPath))
                {
                    throw new InvalidOperationException("Path to bin directiory is null.");
                }

                var catalogPath = Path.Combine(binPath, "Content", "Schemes");

                if (!Directory.Exists(catalogPath))
                {
                    throw new InvalidOperationException($"Scheme catalog \"{catalogPath}\" was not found.");
                }

                var schemeLocator = new FileSchemeLocator(catalogPath);

                return schemeLocator;
            }));

            container.AddSingleton<ISchemeService, SchemeService>();

            container.AddSingleton<ISchemeServiceHandlerFactory, SchemeServiceHandlerFactory>();
        }
    }
}