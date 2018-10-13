using Zilon.Core.Common;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common
{
    public class TestTacticalActStatsSubScheme : SubSchemeBase, ITacticalActStatsSubScheme
    {
        public TestTacticalActStatsSubScheme() : base()
        {
            Offence = new TacticalActOffenceSubScheme();
            Effect = TacticalActEffectType.Damage;
            Efficient = new Roll(3, 1);
            Range = new Range<int>(1, 1);
            HitCount = 1;
            IsMelee = true;
        }

        public TacticalActEffectType Effect { get; }
        public Roll Efficient { get; set; }
        public int HitCount { get; }
        public bool IsMelee { get; }
        public ITacticalActOffenceSubScheme Offence { get; set; }
        public Range<int> Range { get; set; }
    }
}
