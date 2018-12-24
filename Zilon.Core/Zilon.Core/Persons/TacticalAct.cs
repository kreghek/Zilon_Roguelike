using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

using Zilon.Core.Common;
using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Тактическое действие актёров под управлением игрока.
    /// </summary>
    public class TacticalAct : ITacticalAct
    {
        [ExcludeFromCodeCoverage]
        public TacticalAct([NotNull]ITacticalActScheme scheme,
            [NotNull] Roll efficient,
            [NotNull] Roll toHit,
            [CanBeNull] Equipment equipment)
        {
            Scheme = scheme ?? throw new System.ArgumentNullException(nameof(scheme));

            Stats = scheme.Stats ?? throw new System.ArgumentNullException(nameof(scheme));

            Efficient = efficient ?? throw new System.ArgumentNullException(nameof(efficient));

            ToHit = toHit ?? throw new System.ArgumentNullException(nameof(toHit));

            Equipment = equipment;

            Constrains = scheme.Constrains;
        }

        public ITacticalActStatsSubScheme Stats { get; }

        public ITacticalActScheme Scheme { get; }

        public Roll Efficient { get; }

        public Roll ToHit { get; }

        public Equipment Equipment { get; }
        public ITacticalActConstrainsSubScheme Constrains { get; }
    }
}
