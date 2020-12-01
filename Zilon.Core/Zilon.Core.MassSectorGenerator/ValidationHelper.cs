using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Zilon.CommonUtilities;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.MassSectorGenerator
{
    internal static class ValidationHelper
    {
        public static Task CheckSectorAsync(ISectorValidator[] validators, IServiceProvider scopeContainer,
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

        public static SectorSchemeResult GetSectorScheme(Random _random, string[] args, ISchemeService schemeService)
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
    }
}