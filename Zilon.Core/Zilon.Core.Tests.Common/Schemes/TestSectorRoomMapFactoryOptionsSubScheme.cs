using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    public class TestSectorRoomMapFactoryOptionsSubScheme : ISectorRoomMapFactoryOptionsSubScheme
    {
        public int RegionCount { get; set; }
        public int RegionSize { get; set; }
        public SchemeSectorMapGenerator MapGenerator { get => SchemeSectorMapGenerator.Room; }
    }
}