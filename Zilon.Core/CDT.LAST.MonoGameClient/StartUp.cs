using System;
using System.IO;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Bot.Players.NetCore.DependencyInjectionExtensions;
using Zilon.Core.Schemes;
using Zilon.Emulation.Common;

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
            serviceCollection.RegisterBot();
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