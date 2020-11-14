namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    /// <summary>
    ///     Base implemenetation on min region count rule.
    /// </summary>
    internal class RegionCountRule : IRegionMinCountRule
    {
        public int Count { get; set; }
        public string Name => "Minimum region count rule";

        public string Description =>
            "Rule allow to create map with regions to set each transition from map to other map in separated regions.";
    }
}