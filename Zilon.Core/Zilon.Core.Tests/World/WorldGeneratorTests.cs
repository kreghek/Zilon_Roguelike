using System;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Schemes;
using Zilon.Core.Tests.Common;
using Zilon.Core.World;

namespace Zilon.Core.Tests.WorldGeneration
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [Category(TestCategories.REAL_RESOURCE)]
    public class WorldGeneratorTests
    {
        /// <summary>
        /// Тест проверяет, что при создании мира не происходит исключений.
        /// </summary>
        /// <returns></returns>
        [Test]
        [Category(TestCategories.LONG_RUN)]
        public async Task GenerateAsync_FixedDice_NoExceptions()
        {
            var dice = new LinearDice(1);
            var schemeService = CreateSchemeService();
            var generator = new WorldGenerator(dice, schemeService);

            var result = await generator.GenerateGlobeAsync();
            //TODO Вынести в отдельное приложение.
            // Суть приложения - генерировать мир и просматривать результат и историю генерации.
            //result.Globe.Save(@"c:\worldgen");

            result.Should().NotBeNull();
        }

        [Ignore("Эти тесты для ручной проверки. Нужно их привести к нормальным модульным тестам.")]
        [Test]
        public async Task GenerateAsync_ShowHistory()
        {
            var dice = new LinearDice();
            var schemeService = CreateSchemeService();
            var generator = new WorldGenerator(dice, schemeService);

            var result = await generator.GenerateGlobeAsync().ConfigureAwait(false);

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
            var dice = new LinearDice();
            var schemeService = CreateSchemeService();
            var generator = new WorldGenerator(dice, schemeService);

            var result = await generator.GenerateGlobeAsync();

            var region = generator.GenerateRegionAsync(result.Globe, result.Globe.Localities.First().Cell);
        }

        [Test]
        public async Task GenerateRegionAsync_StartProvince_RegionHasStartNode()
        {
            // ARRANGE
            var dice = new LinearDice(1);  // Для тестов указываем кость с фиксированным зурном рандома.
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
            var schemeLocator = FileSchemeLocator.CreateFromEnvVariable();

            var schemeHandlerFactory = new SchemeServiceHandlerFactory(schemeLocator);

            return new SchemeService(schemeHandlerFactory);
        }
    }
}