using Zilon.Core.Common;
using Zilon.Core.Components;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    public class TestPropArmorItemSubScheme : IPropArmorItemSubScheme
    {
        public ImpactType Impact { get; set; }
        public PersonRuleLevel AbsorbtionLevel { get; set; }
        public int ArmorRank { get; set; }
    }
}
