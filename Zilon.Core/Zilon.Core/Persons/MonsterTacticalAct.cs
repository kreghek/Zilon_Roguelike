using Zilon.Core.Common;
using Zilon.Core.Props;
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
        public Roll ToHit { get; }
        public Equipment Equipment => null;
        public ITacticalActConstrainsSubScheme Constrains => null;

        public ITacticalActScheme Scheme { get; }

        public MonsterTacticalAct(ITacticalActStatsSubScheme stats)
        {
            Stats = stats;
            Efficient = stats.Efficient;
            ToHit = new Roll(6, 1);
        }
    }
}
