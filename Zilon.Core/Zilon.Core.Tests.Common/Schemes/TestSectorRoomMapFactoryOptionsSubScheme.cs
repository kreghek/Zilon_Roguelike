using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    [ExcludeFromCodeCoverage]
    public class TestSectorRoomMapFactoryOptionsSubScheme : ISectorRoomMapFactoryOptionsSubScheme
    {
        public int RegionCount { get; set; }
        public int RegionSize { get; set; }
        public SchemeSectorMapGenerator MapGenerator => SchemeSectorMapGenerator.Room;
    }
}