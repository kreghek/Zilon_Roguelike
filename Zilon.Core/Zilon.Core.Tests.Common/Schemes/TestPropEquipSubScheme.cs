using Zilon.Core.Components;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class TestPropEquipSubScheme : IPropEquipSubScheme
    {
        public EquipmentSlotTypes[] SlotTypes { get; set; }
        public string[] ActSids { get; }
        public IPropArmorItemSubScheme[] Armors { get; set; }
        public PersonRule[] Rules { get; set; }
    }
}