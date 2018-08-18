using Zilon.Core.MapGenerators;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.Common
{
    public class TestGridGenMap : MapBase
    {
        public TestGridGenMap(): this(10)
        {
            
        }

        public TestGridGenMap(int size) {

            var gridGenerator = new GridMapGenerator(size);
            gridGenerator.CreateMap(this);
        }
    }
}
