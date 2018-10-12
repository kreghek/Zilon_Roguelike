using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Тактическое действие для монстров.
    /// </summary>
    public class MonsterTacticalAct : ITacticalAct
    {
        public TacticalActStatsSubScheme Stats { get; }

        public MonsterTacticalAct(TacticalActStatsSubScheme stats)
        {
            Stats = stats;
        }
    }
}
