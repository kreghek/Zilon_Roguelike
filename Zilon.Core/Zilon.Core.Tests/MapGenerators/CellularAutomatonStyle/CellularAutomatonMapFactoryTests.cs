using System.Threading.Tasks;

using NUnit.Framework;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Tactics.Spatial.Visualization;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.MapGenerators.CellularAutomatonStyle.Tests
{
    [TestFixture()]
    public class CellularAutomatonMapFactoryTests
    {
        //[Ignore("Этот тест генерирует изображение и он не проеряет что-то конкретное в изоляции")]
        [Test()]
        public async Task CreateAsyncTestAsync()
        {
            var dice = new Dice();

            var mapFactory = new CellularAutomatonMapFactory(dice);

            var sectorScheme = new TestSectorSubScheme {
                TransSectorSids = new string[] { null }
            };

            var map = await mapFactory.CreateAsync(sectorScheme);

            var drawer = new MapDrawer();

            var bmp = drawer.DrawMap(map);

            bmp.Save(@"c:\world-gen-results\map.bmp");
        }
    }
}