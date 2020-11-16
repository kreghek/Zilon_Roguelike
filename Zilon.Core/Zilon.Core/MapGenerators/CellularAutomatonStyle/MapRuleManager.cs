using System.Collections.Generic;
using System.Linq;

namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    /// <summary>
    /// Base implementation of rule managers.
    /// </summary>
    internal class MapRuleManager : IMapRuleManager
    {
        private readonly List<IMapRule> _list = new List<IMapRule>();

        public void AddRule<T>(T rule) where T : IMapRule
        {
            _list.Add(rule);
        }

        public T GetRuleOrNull<T>() where T : IMapRule
        {
            return _list.OfType<T>()
                        .SingleOrDefault();
        }
    }
}