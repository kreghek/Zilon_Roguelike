using Zilon.Core.Components;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    public class TestPropEquipSubScheme : IPropEquipSubScheme
    {
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public int ApRank { get; }
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public int ArmorRank { get; }
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public float Absorption { get; }
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public string[] ActSids { get; }
        public EquipmentSlotTypes[] SlotTypes { get; set; }
    }
}
