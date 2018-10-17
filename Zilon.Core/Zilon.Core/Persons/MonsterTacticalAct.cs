using Zilon.Core.Common;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Тактическое действие для монстров.
    /// </summary>
    public class MonsterTacticalAct : ITacticalAct
    {
        public ITacticalActStatsSubScheme Stats { get; }
        public Roll Efficient { get; }

        public MonsterTacticalAct(ITacticalActStatsSubScheme stats)
        {
            Stats = stats;
            Efficient = stats.Efficient;
        }
    }
}
