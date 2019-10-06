using JsonSubTypes;

using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    [JsonConverter(typeof(JsonSubtypes), nameof(MapGenerator))]
    [JsonSubtypes.KnownSubType(
        typeof(SectorCellularAutomataMapFactoryOptionsSubScheme),
        nameof(SchemeSectorMapGenerator.CellularAutomaton)
        )]
    public sealed class SectorMapFactoryOptionsSubScheme : ISectorMapFactoryOptionsSubScheme
    {
        public SchemeSectorMapGenerator MapGenerator { get; }
    }
}
