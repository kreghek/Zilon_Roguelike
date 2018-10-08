using Zilon.Core.Common;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common
{
    public class TestTacticalActStatsSubScheme : TacticalActStatsSubScheme
    {
        public TestTacticalActStatsSubScheme(): base(new TacticalActOffenceSubScheme(),
            TacticalActEffectType.Damage,
            new Roll(3, 1),
            new Range<int>(1, 1),
            1,
            true)
        {
        }
    }
}
