using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Zilon.CommonUtilities;
using Zilon.Core.MapGenerators;
using Zilon.Core.Schemes;
using Zilon.Core.World;

namespace Zilon.Core.MassSectorGenerator
{
    internal static class Program
    {
        private static readonly Random _random;

        static Program()
        {
            _random = new Random();
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

        public static async Task Main(string[] args)
        {
            var diceSeed = GetDiceSeed(args);
            var outputPath = GetOutputPath(args);

            var startUp = new Startup(diceSeed);
            var serviceContainer = new ServiceCollection();

            startUp.RegisterServices(serviceContainer);

            var serviceProvider = serviceContainer.BuildServiceProvider();

            var schemeService = serviceProvider.GetRequiredService<ISchemeService>();

            var sectorSchemeResult = ValidationHelper.GetSectorScheme(_random, args, schemeService);

            var biome = new Biome(sectorSchemeResult.LocationScheme);
            var sectorNode = new SectorNode(biome, sectorSchemeResult.SectorScheme);
            biome.AddNode(sectorNode);

            var nextCount = 3;
            for (var i = 0; i < nextCount; i++)
            {
                var nextSectorNode = new SectorNode(biome, sectorSchemeResult.SectorScheme);
                biome.AddNode(nextSectorNode);
                biome.AddEdge(sectorNode, nextSectorNode);
            }
                
            var sectorFactory = serviceProvider.GetRequiredService<ISectorGenerator>();
            var sector = await sectorFactory.GenerateAsync(sectorNode).ConfigureAwait(false);
            sector.Scheme = sectorSchemeResult.LocationScheme;

            // Проверка

            var sectorValidators = ValidatorCollector.GetValidatorsInAssembly();
            var checkTask = ValidationHelper.CheckSectorAsync(sectorValidators, serviceProvider, sector);

            var saveTask = ImageHelper.SaveMapAsImageAsync(outputPath, sector);

            await Task.WhenAll(checkTask, saveTask).ConfigureAwait(false);
        }
    }
}