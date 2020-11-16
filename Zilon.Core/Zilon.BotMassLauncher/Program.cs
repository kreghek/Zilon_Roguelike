using System.IO;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Zilon.BotMassLauncher
{
    internal static class Program
    {
        private static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables();
        }

        private static void Main(string[] args)
        {
            var configBuilder = new ConfigurationBuilder();
            BuildConfig(configBuilder);

            var configuration = configBuilder.Build();

            var host = Host
                .CreateDefaultBuilder()
                .ConfigureServices((context, services) => services
                .AddLogging(loggingBuilder => loggingBuilder.AddConsole().AddConfiguration(configuration))
                )
                .Build();

            var workload = ActivatorUtilities.CreateInstance<Workload>(host.Services);

            workload.Run(args);
        }
    }
}