namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    interface IRegionMinCountRule : IMapRule
    {
        int Count { get; set; }
    }
}