using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    public class SectorRoomMapFactoryOptionsSubScheme : SectorMapFactoryOptionsSubSchemeBase, ISectorRoomMapFactoryOptionsSubScheme
    {
        public override SchemeSectorMapGenerator MapGenerator => SchemeSectorMapGenerator.Room;

        [JsonProperty]
        public int RegionCount { get; private set; }

        [JsonProperty]
        public int RegionSize { get; private set; }
    }
}