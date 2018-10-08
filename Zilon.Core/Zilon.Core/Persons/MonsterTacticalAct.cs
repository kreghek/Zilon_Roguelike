using System;
using Zilon.Core.Common;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Тактическое действие для монстров.
    /// </summary>
    public class MonsterTacticalAct : ITacticalAct
    {
        public TacticalActStatsSubScheme Stats { get; }

        public Roll Efficient { get; }

        public MonsterTacticalAct(TacticalActStatsSubScheme stats)
        {
            Stats = stats;
            Efficient = Stats.Efficient;
        }
    }
}
