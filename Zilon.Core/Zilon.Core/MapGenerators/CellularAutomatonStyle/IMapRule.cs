namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    interface IMapRule
    {
        /// <summary>
        /// Name for debug
        /// </summary>
        string Name { get; }

        string Description { get; }
    }
}