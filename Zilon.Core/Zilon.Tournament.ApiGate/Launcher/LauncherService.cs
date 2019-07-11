using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Zilon.Tournament.ApiGate.Launcher
{
    public class LauncherService: BackgroundService
    {
        private readonly ILogger<LauncherService> _logger;
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _configuration;

        public LauncherService(ILogger<LauncherService> logger, IHostingEnvironment env, IConfiguration configuration)
        {
            _logger = logger;
            _env = env;
            _configuration = configuration;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Launcher Service is starting");

            var appPath = "C:\\PROJECTS\\Zilon_Roguelike\\Zilon.Core\\Zilon.Tournament.ApiGate\\bin\\Debug\\netcoreapp2.2\\";
            var schemeCatalogPath = Environment.GetEnvironmentVariable("SCHEME_CATALOG_PATH");
            var outputCatalog = Environment.GetEnvironmentVariable("BOT_OUTPUT_CATALOG");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var process = new Process())
                    {
                        process.StartInfo = new ProcessStartInfo
                        {
                            FileName = $"{appPath}Zilon.BotMassLauncher.exe",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            Arguments = $"parallel=10 mode=duncan env=\"{appPath}Zilon.BotEnvironment.exe\" launchCount=1000 output=\"{outputCatalog}\" schemeCatalogPath=\"{schemeCatalogPath}\"",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        };

                        process.Start();

                        var output = process.StandardOutput.ReadToEnd();
                        var error = process.StandardError.ReadToEnd();

                        process.WaitForExit(30000);
                    }
                }
                catch (Exception e)
                {
                }
            }
            _logger.LogDebug("Launcher service is stopping");

            return Task.CompletedTask;
        }
    }
}
