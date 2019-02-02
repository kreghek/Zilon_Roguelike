using System.Configuration;
using System.Threading.Tasks;
using NUnit.Framework;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Schemes;

namespace Zilon.Core.WorldGeneration.Tests
{
    [TestFixture]
    public class GeneratorTests
    {
        [Test]
        public async Task GenerateTestAsync()
        {
            var dice = new Dice();
            var schemeService = CreateSchemeService();
            var generator = new WorldGenerator(dice, schemeService);

            var globe = await generator.GenerateGlobeAsync();
            globe.Save(@"c:\worldgen");
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