namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    interface IMapRuleManager
    {
        T GetRuleOrNull<T>() where T : IMapRule;

        void AddRule<T>(T rule) where T : IMapRule;
    }
}