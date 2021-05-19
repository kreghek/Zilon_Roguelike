using System;
using System.IO;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Bot.Players;
using Zilon.Bot.Players.NetCore;
using Zilon.Bot.Players.NetCore.DependencyInjectionExtensions;
using Zilon.Bot.Players.Strategies;
using Zilon.Core.Schemes;
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

            serviceCollection.AddSingleton<IHumanActorTaskSource, HumanActorTaskSource>();
            serviceCollection.AddSingleton<IActorTaskSource>(provider =>
                provider.GetRequiredService<IHumanActorTaskSource>());
        }

        protected override void RegisterSchemeService(IServiceCollection container)
        {
            container.AddSingleton((Func<IServiceProvider, ISchemeLocator>)(factory =>
            {
                var mainModule = System.Diagnostics.Process.GetCurrentProcess().MainModule;
                if (mainModule is null)
                {
                    throw new InvalidOperationException("Error during main module calculation.");
                }

                var gamePath = mainModule.FileName;
                if (gamePath is null)
                {
                    throw new InvalidOperationException("Error during current path calculation.");
                }

                var gamePathFileInfo = new FileInfo(gamePath);
                var contentDirectory = gamePathFileInfo.Directory?.FullName!;
                var catalogPath = Path.Combine(contentDirectory, "Content", "Schemes");

                var schemeLocator = new FileSchemeLocator(catalogPath);

                return schemeLocator;
            }));

            container.AddSingleton<ISchemeService, SchemeService>();

            container.AddSingleton<ISchemeServiceHandlerFactory, SchemeServiceHandlerFactory>();
        }
    }
}