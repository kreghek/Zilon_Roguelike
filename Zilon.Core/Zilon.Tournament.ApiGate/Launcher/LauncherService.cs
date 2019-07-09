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
                
            }
            _logger.LogDebug("Launcher service is stopping");
        }
    }
}
