namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    /// <summary>
    ///     Map restriction. For example, min region count.
    /// </summary>
    internal interface IMapRule
    {
        /// <summary>
        ///     Name for debug
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Description for debugging.
        /// </summary>
        string Description { get; }
    }
}