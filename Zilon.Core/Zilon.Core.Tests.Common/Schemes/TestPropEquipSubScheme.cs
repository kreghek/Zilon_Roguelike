using Zilon.Core.Components;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class TestPropEquipSubScheme : IPropEquipSubScheme
    {
        public string[] ActSids { get; }
        public EquipmentSlotTypes[] SlotTypes { get; set; }
        public IPropArmorItemSubScheme[] Armors { get; set; }
        public PersonRule[] Rules { get; set; }
    }
}