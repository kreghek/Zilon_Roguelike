using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Common;
using Zilon.Core.Components;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    [ExcludeFromCodeCoverage]
    public class TestTacticalActStatsSubScheme : SubSchemeBase, ITacticalActStatsSubScheme
    {
        public TestTacticalActStatsSubScheme()
        {
            Offence = new TacticalActOffenceSubScheme();
            Effect = TacticalActEffectType.Damage;
            Efficient = new Roll(3, 1);
            Range = new Range<int>(1, 1);
            HitCount = 1;
            IsMelee = true;
            Targets = TacticalActTargets.Enemy;
        }

        /// <inheritdoc />
        public TacticalActEffectType Effect { get; set; }

        /// <inheritdoc />
        public Roll Efficient { get; set; }

        /// <inheritdoc />
        public int HitCount { get; }

        /// <inheritdoc />
        public bool IsMelee { get; }

        /// <inheritdoc />
        public ITacticalActOffenceSubScheme Offence { get; set; }

        /// <inheritdoc />
        public Range<int> Range { get; set; }

        /// <inheritdoc />
        public TacticalActTargets Targets { get; set; }

        /// <inheritdoc />
        public string[] Tags { get; }

        /// <inheritdoc />
        public float? Duration { get; }

        /// <inheritdoc />
        public CombatActRule[] Rules { get; }
    }
}