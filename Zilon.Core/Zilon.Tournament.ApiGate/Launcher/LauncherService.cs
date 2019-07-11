using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Zilon.Tournament.ApiGate.Launcher
{
    public class LauncherService: BackgroundService
    {
        private readonly ILogger<LauncherService> _logger;
        private readonly IHostingEnvironment _env;

        public LauncherService(ILogger<LauncherService> logger, IHostingEnvironment env)
        {
            _logger = logger;
            _env = env;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Launcher Service is starting");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var process = new Process())
                    {
                        process.StartInfo = new ProcessStartInfo
                        {
                            FileName = "C:\\PROJECTS\\Zilon_Roguelike\\Zilon.Core\\Zilon.Tournament.ApiGate\\bin\\Debug\\netcoreapp2.2\\Zilon.BotMassLauncher.exe",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            Arguments = "parallel=10 mode=duncan env=\"C:\\PROJECTS\\Zilon_Roguelike\\Zilon.Core\\Zilon.Tournament.ApiGate\\bin\\Debug\\netcoreapp2.2\\Zilon.BotEnvironment.exe\" launchCount=1 schemeCatalogPath=\"C:\\PROJECTS\\Zilon_Roguelike\\Zilon.Client\\Assets\\Resources\\Schemes\"",
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
