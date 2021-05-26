using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    public sealed class SectorCellularAutomataMapFactoryOptionsSubScheme : SectorMapFactoryOptionsSubSchemeBase,
        ISectorCellularAutomataMapFactoryOptionsSubScheme
    {
        public override SchemeSectorMapGenerator MapGenerator => SchemeSectorMapGenerator.CellularAutomaton;

        [JsonProperty]
        public int MapWidth { get; private set; }

        [JsonProperty]
        public int MapHeight { get; private set; }

        [JsonProperty]
        public int ChanceToStartAlive { get; private set; }
    }

    public sealed class SectorOpenMapFactoryOptionsSubScheme : SectorMapFactoryOptionsSubSchemeBase, ISectorOpenMapFactoryOptionsSubScheme
    {
        public override SchemeSectorMapGenerator MapGenerator => SchemeSectorMapGenerator.Open;

        [JsonProperty]
        public int Size { get; private set; }
    }
}