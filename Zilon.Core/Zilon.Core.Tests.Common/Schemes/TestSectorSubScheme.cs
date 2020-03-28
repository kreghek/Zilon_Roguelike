using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    public sealed class TestSectorSubScheme : ISectorSubScheme
    {
        public TestSectorSubScheme()
        {
            RegionMonsterCount = 5;
        }

        public string Sid { get; set; }
        public LocalizedStringSubScheme Name { get; set; }
        public LocalizedStringSubScheme Description { get; set; }
        public string[] RegularMonsterSids { get; set; }
        public string[] RareMonsterSids { get; set; }
        public string[] ChampionMonsterSids { get; set; }
        public int RegionMonsterCount { get; set; }
        public int TotalChestCount { get; set; }
        public string[] ChestDropTableSids { get; set; }
        public ISectorTransitionSubScheme[] TransSectorSids { get; set; }
        public bool IsStart { get; set; }
        public int RegionChestCountRatio { get; set; }
        public int MinRegionMonsterCount { get; }
        public SchemeSectorMapGenerator MapGenerator { get; }
        public ISectorMapFactoryOptionsSubScheme MapGeneratorOptions { get; set; }

        public override string ToString()
        {
            return Sid;
        }
    }
}
