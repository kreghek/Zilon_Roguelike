using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Schemes;

namespace Zilon.Core.MapGenerators.OpenStyle.Tests
{
    [TestFixture]
    public class OpenMapFactoryTests
    {
        [Test]
        public async Task CreateAsyncTestAsync()
        {
            // ARRANGE
            var dice = Mock.Of<IDice>(x => x.Roll(It.IsAny<int>()) == 1);
            var mapFactory = new OpenMapFactory(dice);
            var options = new SectorMapFactoryOptions(new TestSectorOpenMapFactoryOptionsSubScheme { Size = 1001 });

            // ACT
            var map = await mapFactory.CreateAsync(options);

            // ASSERT
            Assert.Pass();
        }

        public sealed class TestSectorOpenMapFactoryOptionsSubScheme : SectorMapFactoryOptionsSubSchemeBase,
            ISectorOpenMapFactoryOptionsSubScheme
        {
            public override SchemeSectorMapGenerator MapGenerator => SchemeSectorMapGenerator.Open;

            public int Size { get; set; }
        }
    }
}