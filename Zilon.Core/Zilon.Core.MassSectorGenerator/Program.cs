using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Zilon.CommonUtilities;
using Zilon.Core.MapGenerators;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.World;

namespace Zilon.Core.MassSectorGenerator
{
    internal class Program
    {
        private static readonly Random _random;

        static Program()
        {
            _random = new Random();
        }

        private static async Task Main(string[] args)
        {
            var diceSeed = GetDiceSeed(args);
            var outputPath = GetOutputPath(args);

            var startUp = new Startup(diceSeed);
            var serviceContainer = new ServiceCollection();

            startUp.RegisterServices(serviceContainer);

            var serviceProvider = serviceContainer.BuildServiceProvider();

            var schemeService = serviceProvider.GetRequiredService<ISchemeService>();

            var sectorSchemeResult = GetSectorScheme(args, schemeService);

            var biome = new Biome(sectorSchemeResult.Location);
            var sectorNode = new SectorNode(biome, sectorSchemeResult.Sector);

            var sectorFactory = serviceProvider.GetRequiredService<ISectorGenerator>();
            var sector = await sectorFactory.GenerateAsync(sectorNode).ConfigureAwait(false);
            sector.Scheme = sectorSchemeResult.Location;

            // Проверка

            var sectorValidators = GetValidatorsInAssembly();
            var checkTask = CheckSectorAsync(sectorValidators, serviceProvider, sector);

            var saveTask = SaveMapAsImageAsync(outputPath, sector);

            await Task.WhenAll(checkTask, saveTask).ConfigureAwait(false);
        }

        private static ISectorValidator[] GetValidatorsInAssembly()
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            var validatorTypes = thisAssembly.GetTypes()
                .Where(x => typeof(ISectorValidator).IsAssignableFrom(x))
                .Where(x => !x.IsInterface && !x.IsAbstract);

            var validators = validatorTypes.Select(x => Activator.CreateInstance(x)).Cast<ISectorValidator>();

            return validators.ToArray();
        }

        private static Task SaveMapAsImageAsync(string outputPath, ISector sector)
        {
            if (outputPath != null)
            {
                SaveMapAsImage(sector.Map, outputPath);
            }

            return Task.CompletedTask;
        }

        private static Task CheckSectorAsync(ISectorValidator[] validators, IServiceProvider scopeContainer,
            ISector sector)
        {
            return Task.Run(() =>
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();

                var checkTasks = validators.Select(x => x.Validate(sector, scopeContainer));

                var allTasks = Task.WhenAll(checkTasks);

                try
                {
                    allTasks.Wait();
                }
                catch (AggregateException exception)
                {
                    Log.Error("Сектор содержит ошибки:");

                    foreach (var inner in exception.InnerExceptions)
                    {
                        if (inner is SectorValidationException)
                        {
                            Log.Error(inner);
                        }
                    }

                    throw;
                }

                stopWatch.Stop();

                Log.Info($"CHECK DURATION: {stopWatch.Elapsed.TotalSeconds} SEC");
            });
        }

        private static string GetOutputPath(string[] args)
        {
            var outputPath = ArgumentHelper.GetProgramArgument(args, Args.OUT_PATH_ARG_NAME);
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                return null;
            }

            try
            {
                Path.GetFullPath(outputPath);
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                return null;
            }

            return outputPath;
        }

        private static int GetDiceSeed(string[] args)
        {
            var diceSeedString = ArgumentHelper.GetProgramArgument(args, Args.DICE_SEED_ARG_NAME);

            int diceSeed;
            if (string.IsNullOrWhiteSpace(diceSeedString))
            {
                diceSeed = _random.Next(0, int.MaxValue);
            }
            else
            {
                if (!int.TryParse(diceSeedString, out diceSeed))
                {
                    throw new SectorGeneratorException($"Зерно рандома задано некорректно: {diceSeedString}.");
                }
            }

            Log.Info($"DICE SEED: {diceSeed}");

            return diceSeed;
        }

        private static SectorSchemeResult GetSectorScheme(string[] args, ISchemeService schemeService)
        {
            var locationSchemeSid = ArgumentHelper.GetProgramArgument(args, Args.LOCATION_SCHEME_SID_ARG_NAME);
            var sectorSchemeSid = ArgumentHelper.GetProgramArgument(args, Args.SECTOR_SCHEME_SID_ARG_NAME);
            if (string.IsNullOrWhiteSpace(locationSchemeSid) && string.IsNullOrWhiteSpace(sectorSchemeSid))
            {
                // Если схемы не указаны, то берём случайную схему.
                // Это используется на билд-сервере, чтобы случайно проверить несколько схем.

                var locationSchemes = schemeService.GetSchemes<ILocationScheme>()
                    .Where(x => x.SectorLevels != null && x.SectorLevels.Any())
                    .ToArray();
                var locationSchemeIndex = _random.Next(0, locationSchemes.Length);
                var locationScheme = locationSchemes[locationSchemeIndex];

                var sectorSchemes = locationScheme.SectorLevels;
                var sectorSchemeIndex = _random.Next(0, sectorSchemes.Length);
                var sectorScheme = sectorSchemes[sectorSchemeIndex];

                Log.Info($"SCHEME: {locationScheme.Sid} - {sectorScheme.Sid}(index:{sectorSchemeIndex})");

                var result = new SectorSchemeResult(locationScheme, sectorScheme);

                return result;
            }
            else
            {
                // Если схемы заданы, то строим карту на их основе.
                // Это будет использовано для отладки.

                var locationScheme = schemeService.GetScheme<ILocationScheme>(locationSchemeSid);
                if (locationScheme == null)
                {
                    throw new SectorGeneratorException($"Не найдена схема локации {locationSchemeSid}.");
                }

                var sectorScheme = locationScheme.SectorLevels.SingleOrDefault(x => x.Sid == sectorSchemeSid);
                if (sectorScheme == null)
                {
                    throw new SectorGeneratorException($"Не найдена схема сектора {sectorSchemeSid}.");
                }

                var result = new SectorSchemeResult(locationScheme, sectorScheme);

                return result;
            }
        }

        private static void SaveMapAsImage(ISectorMap map, string outputPath)
        {
            using (var bmp = MapDrawer.DrawMap(map))
            {
                bmp.Save(outputPath);
            }
        }
    }
}