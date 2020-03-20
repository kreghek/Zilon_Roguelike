using Zilon.Core.Schemes;

namespace Zilon.Core.MapGenerators.RoomStyle
{
    public class TownSectorScheme : SchemeBase, ISectorSubScheme
    {
        public string[] RegularMonsterSids { get; }
        public string[] RareMonsterSids { get; }
        public string[] ChampionMonsterSids { get; }
        public int RegionCount { get; set; }
        public int RegionSize { get; set; }
        public int RegionMonsterCount { get; }
        public int TotalChestCount { get; }
        public int RegionChestCountRatio { get; }
        public string[] ChestDropTableSids { get; }
        public string[] TransSectorSids { get; }
        public bool IsStart { get; }
        public int MinRegionMonsterCount { get; }
        public ISectorMapFactoryOptionsSubScheme MapGeneratorOptions { get; }
    }
}
