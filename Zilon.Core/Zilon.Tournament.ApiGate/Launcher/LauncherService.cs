using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Zilon.Tournament.ApiGate.Launcher
{
    public class LauncherService: BackgroundService
    {
        private readonly ILogger<LauncherService> _logger;
        
        public LauncherService(ILogger<LauncherService> logger)
        {
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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
                            FileName = @".\Zilon.BotMassLauncher.exe",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            Arguments = "parallel=10 mode=duncan env=Zilon.BotEnvironment.exe launchCount=1"
                        };

                        process.Start();

                        process.WaitForExit(30000);
                    }
                }
                catch (Exception e)
                {
                }
            }
            _logger.LogDebug("Launcher service is stopping");
        }
    }
}
