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
        public async Task GenerateAsyncTest()
        {
            var dice = new Dice();
            var schemeService = CreateSchemeService();
            var generator = new WorldGenerator(dice, schemeService);

            var globe = await generator.GenerateGlobeAsync();
            globe.Save(@"c:\worldgen");
        }

        [Ignore("Эти тесты для ручной проверки. Нужно их привести к нормальным модульным тестам.")]
        [Test()]
        public async Task GenerateRegionAsyncTest()
        {
            var dice = new Dice();
            var schemeService = CreateSchemeService();
            var generator = new WorldGenerator(dice, schemeService);

            var globe = await generator.GenerateGlobeAsync();

            var region = generator.GenerateRegionAsync(globe, globe.Localities.First().Cell);
        }

        [Test]
        public async Task GenerateRegionAsync_StartProvince_RegionHasStartNode()
        {
            // ARRANGE
            var dice = new Dice(1);  // Для тестов указываем кость с фиксированным зурном рандома.
            var schemeService = CreateSchemeService();
            var generator = new WorldGenerator(dice, schemeService);

            var globe = await generator.GenerateGlobeAsync();


            // ACT
            var region = await generator.GenerateRegionAsync(globe, globe.StartProvince);



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