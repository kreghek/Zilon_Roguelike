using Zilon.Core.Components;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    public class TestPropEquipSubScheme : IPropEquipSubScheme
    {
        public float Power { get; }
        public int ApRank { get; }
        public int ArmorRank { get; }
        public float Absorbtion { get; }
        public string[] ActSids { get; }
        public EquipmentSlotTypes[] SlotTypes { get; set; }
    }
}
