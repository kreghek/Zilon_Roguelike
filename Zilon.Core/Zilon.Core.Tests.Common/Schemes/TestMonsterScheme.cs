using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    public class TestMonsterScheme : SchemeBase, IMonsterScheme
    {
        public IMonsterDefenceSubScheme Defence { get; }
        public string[] DropTableSids { get; }
        public int Hp { get; set; }
        public ITacticalActStatsSubScheme PrimaryAct { get; set; }
    }
}
