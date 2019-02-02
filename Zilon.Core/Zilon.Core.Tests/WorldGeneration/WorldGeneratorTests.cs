using NUnit.Framework;
using Zilon.Core.WorldGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zilon.Core.Schemes;
using System.Configuration;
using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration.Tests
{
    [TestFixture()]
    public class WorldGeneratorTests
    {
        [Test]
        public async Task GenerateAsyncTest()
        {
            var dice = new Dice();
            var schemeService = CreateSchemeService();
            var generator = new WorldGenerator(dice, schemeService);

            var globe = await generator.GenerateGlobeAsync();
            globe.Save(@"c:\worldgen");
        }

        [Test()]
        public async Task GenerateRegionAsyncTest()
        {
            var dice = new Dice();
            var schemeService = CreateSchemeService();
            var generator = new WorldGenerator(dice, schemeService);

            var globe = await generator.GenerateGlobeAsync();

            var region = generator.GenerateRegionAsync(globe, globe.Localities.First().Cell);
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