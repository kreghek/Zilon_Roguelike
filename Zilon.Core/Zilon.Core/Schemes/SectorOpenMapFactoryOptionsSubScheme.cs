using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    public sealed class SectorOpenMapFactoryOptionsSubScheme : SectorMapFactoryOptionsSubSchemeBase,
        ISectorOpenMapFactoryOptionsSubScheme
    {
        public override SchemeSectorMapGenerator MapGenerator => SchemeSectorMapGenerator.Open;

        [JsonProperty]
        public int Size { get; private set; }
    }
}