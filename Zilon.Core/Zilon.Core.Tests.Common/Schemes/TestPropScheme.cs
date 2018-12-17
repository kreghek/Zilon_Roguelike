using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    public class TestPropScheme : SchemeBase, IPropScheme
    {
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public CraftSubScheme Craft { get; }
        public IPropEquipSubScheme Equip { get; set; }
        public IPropUseSubScheme Use { get; set; }
        public string[] Tags { get; set; }
        public IPropBulletSubScheme Bullet { get; }
    }
}
