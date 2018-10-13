using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common
{
    public class TestTacticalActScheme : SchemeBase, ITacticalActScheme
    {
        public TacticalActConstrainsSubScheme Constrains { get; set; }
        public ITacticalActStatsSubScheme Stats { get; set; }
    }
}
