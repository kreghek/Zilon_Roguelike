namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    /// <summary>
    /// Service to collect rules and pass they to other services.
    /// </summary>
    interface IMapRuleManager
    {
        T GetRuleOrNull<T>() where T : IMapRule;

        void AddRule<T>(T rule) where T : IMapRule;
    }
}