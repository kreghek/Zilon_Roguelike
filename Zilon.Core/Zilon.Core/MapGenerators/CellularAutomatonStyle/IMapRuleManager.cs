namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    /// <summary>
    /// Service to collect rules and pass they to other services.
    /// </summary>
    internal interface IMapRuleManager
    {
        void AddRule<T>(T rule) where T : IMapRule;
        T GetRuleOrNull<T>() where T : IMapRule;
    }
}