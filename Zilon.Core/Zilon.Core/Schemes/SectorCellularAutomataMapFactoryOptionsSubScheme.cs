using Newtonsoft.Json;

namespace Zilon.Core.Schemes
{
    public class SectorCellularAutomataMapFactoryOptionsSubScheme : SectorMapFactoryOptionsSubSchemeBase,
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
}