using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    public class TestMonsterScheme : SchemeBase, IMonsterScheme
    {
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public IMonsterDefenseSubScheme Defense { get; }
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public string[] DropTableSids { get; }
        public int Hp { get; set; }
        public ITacticalActStatsSubScheme PrimaryAct { get; set; }
    }
}
