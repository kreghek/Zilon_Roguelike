using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Тактическое действие для монстров.
    /// </summary>
    public class MonsterTacticalAct : ITacticalAct
    {
        public ITacticalActStatsSubScheme Stats { get; }

        public MonsterTacticalAct(ITacticalActStatsSubScheme stats)
        {
            Stats = stats;
        }
    }
}
