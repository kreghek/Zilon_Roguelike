using Zilon.Core.MapGenerators;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tests.Common
{
    /// <summary>
    /// Тестовый класс карты со стенами.
    /// </summary>
    public class TestGridGenWallMap : MapBase
    {
        public TestGridGenWallMap()
        {
            var gridGenerator = new GridMapGenerator(10);
            gridGenerator.CreateMap(this);

            this.RemoveEdge(3, 3, 3, 4);
            this.RemoveEdge(3, 3, 2, 3);
        }
    }
}
