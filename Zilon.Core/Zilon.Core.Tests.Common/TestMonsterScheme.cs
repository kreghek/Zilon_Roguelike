using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common
{
    public class TestMonsterScheme : SchemeBase, IMonsterScheme
    {
        public IMonsterDefenceScheme Defence { get; }
        public string[] DropTableSids { get; }
        public int Hp { get; set; }
        public ITacticalActStatsSubScheme PrimaryAct { get; set; }
    }
}
