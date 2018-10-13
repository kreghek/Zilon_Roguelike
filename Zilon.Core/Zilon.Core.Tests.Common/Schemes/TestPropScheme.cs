using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    public class TestPropScheme : SchemeBase, IPropScheme
    {
        public CraftSubScheme Craft { get; }
        public IPropEquipSubScheme Equip { get; set; }
        public IPropUseSubScheme Use { get; set; }
    }
}
