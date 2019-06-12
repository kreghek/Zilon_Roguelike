using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Schemes;
using Zilon.Core.WorldGeneration;

namespace Zilon.Core.Tests.WorldGeneration
{
    [TestFixture()]
    public class WorldGeneratorTests
    {
        [Ignore("Эти тесты для ручной проверки. Нужно их привести к нормальным модульным тестам.")]
        [Test]
        public async Task GenerateAsync_SaveMapToPng()
        {
            var dice = new Dice();
            var schemeService = CreateSchemeService();
            var generator = new WorldGenerator(dice, schemeService);

            var result = await generator.GenerateGlobeAsync();
            result.Globe.Save(@"c:\worldgen");
        }

        [Ignore("Эти тесты для ручной проверки. Нужно их привести к нормальным модульным тестам.")]
        [Test]
        public async Task GenerateAsync_ShowHistory()
        {
            var dice = new Dice();
            var schemeService = CreateSchemeService();
            var generator = new WorldGenerator(dice, schemeService);

            var result = await generator.GenerateGlobeAsync();

            var historyText = string.Empty;

            var iterationHistory = result.History.Items.GroupBy(x => x.Iteration).OrderBy(x => x.Key);

            foreach (var iterationHistoryGroup in iterationHistory)
            {
                historyText += $"{iterationHistoryGroup.Key} iteration" + Environment.NewLine;
                foreach (var historyItem in iterationHistoryGroup)
                {
                    historyText += historyItem.Event + Environment.NewLine;
                }
            }

            Console.WriteLine(historyText);

        }

        [Ignore("Эти тесты для ручной проверки. Нужно их привести к нормальным модульным тестам.")]
        [Test()]
        public async Task GenerateRegionAsyncTest()
        {
            var dice = new Dice();
            var schemeService = CreateSchemeService();
            var generator = new WorldGenerator(dice, schemeService);

            var result = await generator.GenerateGlobeAsync();

            var region = generator.GenerateRegionAsync(result.Globe, result.Globe.Localities.First().Cell);
        }

        [Test]
        public async Task GenerateRegionAsync_StartProvince_RegionHasStartNode()
        {
            // ARRANGE
            var dice = new Dice(1);  // Для тестов указываем кость с фиксированным зурном рандома.
            var schemeService = CreateSchemeService();
            var generator = new WorldGenerator(dice, schemeService);

            var result = await generator.GenerateGlobeAsync();


            // ACT
            var region = await generator.GenerateRegionAsync(result.Globe, result.Globe.StartProvince);



            // ASSERT
            region.RegionNodes.Should().ContainSingle(x => x.IsStart, "В стартовой провинции должен быть один стартовый узел.");
        }

        private ISchemeService CreateSchemeService()
        {
            var schemePath = ConfigurationManager.AppSettings["SchemeCatalog"];

            var schemeLocator = new FileSchemeLocator(schemePath);

            var schemeHandlerFactory = new SchemeServiceHandlerFactory(schemeLocator);

            return new SchemeService(schemeHandlerFactory);
        }
    }
}