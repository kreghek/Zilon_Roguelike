using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    public class TestPropScheme : SchemeBase, IPropScheme
    {
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public new LocalizedStringSubScheme Name { get => base.Name; set => base.Name = value; }

        public CraftSubScheme Craft { get; }

        public IPropEquipSubScheme Equip { get; set; }

        public IPropUseSubScheme Use { get; set; }

        public string[] Tags { get; set; }

        public IPropBulletSubScheme Bullet { get; set; }

        public string IsMimicFor { get; set; }
    }
}