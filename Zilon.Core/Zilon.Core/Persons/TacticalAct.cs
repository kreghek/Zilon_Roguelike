using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Тактическое действие актёров под управлением игрока.
    /// </summary>
    public class TacticalAct : ITacticalAct
    {
        public TacticalAct(ITacticalActScheme scheme)
        {
            Scheme = scheme;
            Stats = scheme.Stats;
        }

        public ITacticalActStatsSubScheme Stats { get; }

        public ITacticalActScheme Scheme { get; }
    }
}
