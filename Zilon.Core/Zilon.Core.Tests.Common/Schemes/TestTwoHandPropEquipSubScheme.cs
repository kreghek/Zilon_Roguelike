namespace Zilon.Core.Tests.Common.Schemes
{
    using Components;

    using Core.Schemes;

    public record TestTwoHandPropEquipSubScheme : IPropEquipSubScheme
    {
        public TestTwoHandPropEquipSubScheme()
        {
            EquipRestrictions = new PropEquipRestrictions
            {
                PropHandUsage = PropHandUsage.TwoHanded
            };
        }

        public EquipmentSlotTypes[] SlotTypes { get; init; }

        public string[] ActSids { get; }

        public IPropArmorItemSubScheme[] Armors { get; init; }

        public PersonRule[] Rules { get; init; }

        public IPropEquipRestrictions EquipRestrictions { get; }
    }
}