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

            string? appPath = Environment.GetEnvironmentVariable("APP_PATH");
            string? schemeCatalogPath = Environment.GetEnvironmentVariable("SCHEME_CATALOG_PATH");
            string? outputCatalog = Environment.GetEnvironmentVariable("BOT_OUTPUT_CATALOG");

            BotInfo[] botInfos = GetAllBots(appPath);

            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (BotInfo botInfo in botInfos)
                {
                    foreach (string mode in botInfo.Modes)
                    {
                        try
                        {
                            using (Process process = new Process())
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
            List<BotInfo> botList = new List<BotInfo>();

            string botRootCatalog = Path.Combine(appPath, "bots");

            IEnumerable<string> botCatalogs = Directory.EnumerateDirectories(botRootCatalog);
            foreach (string botCatalog in botCatalogs)
            {
                string botSettingsFilePath = Path.Combine(botCatalog, "settings.json");
                string botSettingsRaw = File.ReadAllText(botSettingsFilePath);
                var botSettings = JsonConvert.DeserializeObject<BotSettings>(botSettingsRaw);

                DirectoryInfo di = new DirectoryInfo(botCatalog);

                BotInfo botInfo = new BotInfo
                {
                    Catalog = di.Name, Assembly = botSettings.AssemblyName, Modes = botSettings.Modes
                };

                botList.Add(botInfo);
            }

            return botList.ToArray();
        }
    }
}