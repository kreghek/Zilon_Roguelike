namespace Zilon.Core.Tests.Common.Schemes
{
    using System.Diagnostics.CodeAnalysis;

    using Newtonsoft.Json;

    using Components;

    using Core.Schemes;

    [ExcludeFromCodeCoverage]
    public class TestTwoHandPropEquipSubScheme : IPropEquipSubScheme
    {
        public TestTwoHandPropEquipSubScheme()
        {
            const string json = "{ \"PropHandUsage\": \"TwoHanded\" }";
            EquipRestrictions = JsonConvert.DeserializeObject<PropEquipRestrictions>(json);
        }

        public EquipmentSlotTypes[] SlotTypes { get; set; }

        public string[] ActSids { get; }

        public IPropArmorItemSubScheme[] Armors { get; set; }

        public PersonRule[] Rules { get; set; }

        public IPropEquipRestrictions EquipRestrictions { get; }
    }
}