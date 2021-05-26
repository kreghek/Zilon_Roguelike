using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Zilon.Core.MapGenerators.OpenStyle;
using Zilon.Core.Schemes;

namespace Zilon.Core.MapGenerators.OpenStyle.Tests
{
    [TestFixture()]
    public class OpenMapFactoryTests
    {
        [Test()]
        public async Task CreateAsyncTestAsync()
        {
            // ARRANGE
            var mapFactory = new OpenMapFactory();
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