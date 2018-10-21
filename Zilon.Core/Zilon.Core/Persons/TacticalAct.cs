using Zilon.Core.Common;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Тактическое действие актёров под управлением игрока.
    /// </summary>
    public class TacticalAct : ITacticalAct
    {
        public TacticalAct(ITacticalActScheme scheme, Roll efficient, Roll toHit)
        {
            Scheme = scheme ?? throw new System.ArgumentNullException(nameof(scheme));

            Stats = scheme.Stats ?? throw new System.ArgumentNullException(nameof(scheme));

            Efficient = efficient ?? throw new System.ArgumentNullException(nameof(efficient));

            ToHit = toHit ?? throw new System.ArgumentNullException(nameof(toHit));
        }

        public ITacticalActStatsSubScheme Stats { get; }

        public ITacticalActScheme Scheme { get; }
        public Roll Efficient { get; }
        public Roll ToHit { get; }
    }
}
