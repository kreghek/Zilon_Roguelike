namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    /// <summary>
    /// Require minimum region count on map.
    /// </summary>
    interface IRegionMinCountRule : IMapRule
    {
        int Count { get; set; }
    }
}