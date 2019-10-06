using System;
using System.Linq;

using Zilon.CommonUtilities;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators.CellularAutomatonStyle;
using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.Schemes;

namespace Zilon.SectorGegerator
{
    class Program
    {
        private const string DICE_SEED_ARG_NAME = "dice_seed";
        private const string SCHEME_CATALOG_PATH_ARG_NAME = "scheme_catalog";
        private const string LOCATION_SCHEME_SID_ARG_NAME = "location";
        private const string SECTOR_SCHEME_SID_ARG_NAME = "sector";
        private const string OUT_PATH_ARG_NAME = "out";

        static async System.Threading.Tasks.Task Main(string[] args)
        {
            try
            {
                var dice = CreateDice(args);

                var schemeService = CreateSchemeService(args);

                var sectorScheme = GetSectorScheme(args, schemeService);

                var cellularAutomatonMpfactory = new CellularAutomatonMapFactory(dice);

                var roomRandomSource = new RoomGeneratorRandomSource(dice);
                var roomGeneratory = new RoomGenerator(roomRandomSource);
                var roomMapFactory = new RoomMapFactory(roomGeneratory);

                var mapFactorySelector = new MapFactorySelector(cellularAutomatonMpfactory, roomMapFactory);

                var mapFactory = mapFactorySelector.GetMapFactory(sectorScheme);

                var map = await mapFactory.CreateAsync(sectorScheme);

                SaveMapAsImage(args, map);
            }
            catch(SectorGeneratorException exception)
            {
                // Эти исключения более-менее контролируемы.
                Log.Error(exception.Message);
            }
            catch (Exception)
            {
                // Это неконтроллируемые исключения.

                // Мы их не отлавливаем и позволяем приложению упасть.
                // Это нужно, чтобы выполнение на билдсервере прикратилось ошибкой.
                // Так мы узнаем, что была какая-то проблема.

                throw;
            }
        }

        private static void SaveMapAsImage(string[] args, Core.Tactics.Spatial.ISectorMap map)
        {
            var drawer = new MapDrawer();

            var bmp = drawer.DrawMap(map);

            var outPath = ArgumentHelper.GetProgramArgument(args, OUT_PATH_ARG_NAME);

            bmp.Save(outPath);
        }

        private static ISectorSubScheme GetSectorScheme(string[] args, ISchemeService schemeService)
        {
            var random = new Random();

            var locationSchemeSid = ArgumentHelper.GetProgramArgument(args, LOCATION_SCHEME_SID_ARG_NAME);
            var sectorSchemeSid = ArgumentHelper.GetProgramArgument(args, SECTOR_SCHEME_SID_ARG_NAME);
            if (string.IsNullOrWhiteSpace(locationSchemeSid) && string.IsNullOrWhiteSpace(sectorSchemeSid))
            {
                // Если схемы не указаны, то берём случайную схему.
                // Это используется на билд-сервере, чтобы случайно проверить несколько схем.

                var locationSchemes = schemeService.GetSchemes<ILocationScheme>()
                    .Where(x=>x.SectorLevels != null && x.SectorLevels.Any())
                    .ToArray();
                var locationSchemeIndex = random.Next(0, locationSchemes.Length);
                var locationScheme = locationSchemes[locationSchemeIndex];

                var sectorSchemes = locationScheme.SectorLevels;
                var sectorSchemeIndex = random.Next(0, sectorSchemes.Length);
                var sectorScheme = sectorSchemes[sectorSchemeIndex];

                Log.Info($"SCHEME: {locationScheme.Sid} - {sectorScheme.Sid}(index:{sectorSchemeIndex})");

                return sectorScheme;
            }
            else
            {
                // Если схемы заданы, то строим карту на их основе.
                // Это будет использовано для отладки.

                var locationScheme = schemeService.GetScheme<ILocationScheme>(locationSchemeSid);
                var sectorScheme = locationScheme.SectorLevels.Single(x => x.Sid == sectorSchemeSid);
                return sectorScheme;
            }
        }

        private static ISchemeService CreateSchemeService(string[] args)
        {
            var schemeCatalogPath = ArgumentHelper.GetProgramArgument(args, SCHEME_CATALOG_PATH_ARG_NAME);

            var schemeLocator = new FileSchemeLocator(schemeCatalogPath);
            var schemeServiceHandlerFactory = new SchemeServiceHandlerFactory(schemeLocator);
            var schemeService = new SchemeService(schemeServiceHandlerFactory);

            return schemeService;
        }

        private static IDice CreateDice(string[] args)
        {
            var diceSeedString = ArgumentHelper.GetProgramArgument(args, DICE_SEED_ARG_NAME);

            IDice dice;
            if (string.IsNullOrWhiteSpace(diceSeedString))
            {
                dice = new Dice();
            }
            else
            {
                if (int.TryParse(diceSeedString, out var diceSeed))
                {
                    dice = new Dice(diceSeed);
                    Log.Info($"DICE SEED: {diceSeed}");
                }
                else
                {
                    throw new SectorGeneratorException($"Зерно рандома задано некорректно: {diceSeedString}.");
                }
            }

            return dice;
        }
    }
}
