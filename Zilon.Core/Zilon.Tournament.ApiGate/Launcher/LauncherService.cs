using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Zilon.Tournament.ApiGate.Launcher
{
    public class LauncherService : BackgroundService
    {
        private readonly ILogger<LauncherService> _logger;

        public LauncherService(ILogger<LauncherService> logger)
        {
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Launcher Service is starting");

            var appPath = Environment.GetEnvironmentVariable("APP_PATH");
            var schemeCatalogPath = Environment.GetEnvironmentVariable("SCHEME_CATALOG_PATH");
            var outputCatalog = Environment.GetEnvironmentVariable("BOT_OUTPUT_CATALOG");

            var botInfos = GetAllBots(appPath);

            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var botInfo in botInfos)
                {
                    foreach (var mode in botInfo.Modes)
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
                                    Arguments =
                                        $"parallel=10 mode={mode} botCatalog={botInfo.Catalog} botAssembly={botInfo.Assembly} env=\"{appPath}Zilon.BotEnvironment.exe\" launchCount=1000 output=\"{outputCatalog}\" schemeCatalogPath=\"{schemeCatalogPath}\"",
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true
                                };

                                process.Start();

                                // Один бот в массланчере в одном режиме может работать 1 час максимум
                                process.WaitForExit((int)(3.6 * 1000_000));
                            }
                        }
                        catch (Exception exception)
                        {
                            // Просто логируем это исключение.
                            //TODO Нужно детально посмотреть, что можно делать с исключением.
                            // Возможно есть более надёжный и точный способ обработки.
                            Console.WriteLine($"[X] {exception}");
                        }
                    }
                }
            }

            _logger.LogDebug("Launcher service is stopping");

            return Task.CompletedTask;
        }

        private static BotInfo[] GetAllBots(string appPath)
        {
            var botList = new List<BotInfo>();

            var botRootCatalog = Path.Combine(appPath, "bots");

            var botCatalogs = Directory.EnumerateDirectories(botRootCatalog);
            foreach (var botCatalog in botCatalogs)
            {
                var botSettingsFilePath = Path.Combine(botCatalog, "settings.json");
                var botSettingsRaw = File.ReadAllText(botSettingsFilePath);
                var botSettings = JsonConvert.DeserializeObject<BotSettings>(botSettingsRaw);

                var di = new DirectoryInfo(botCatalog);

                var botInfo = new BotInfo
                {
                    Catalog = di.Name, Assembly = botSettings.AssemblyName, Modes = botSettings.Modes
                };

                botList.Add(botInfo);
            }

            return botList.ToArray();
        }
    }
}