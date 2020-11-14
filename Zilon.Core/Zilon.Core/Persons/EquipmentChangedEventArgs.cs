using Zilon.Core.Props;

namespace Zilon.Core.Persons
{
    public class EquipmentChangedEventArgs : EventArgs
    {
        [ExcludeFromCodeCoverage]
        public EquipmentChangedEventArgs(Equipment equipment,
            Equipment oldEquipment,
            int slotIndex)
        {
            Equipment = equipment;
            OldEquipment = oldEquipment;
            SlotIndex = slotIndex;
        }

        [PublicAPI] public Equipment Equipment { get; }

        [PublicAPI] public Equipment OldEquipment { get; }

        [PublicAPI] public int SlotIndex { get; }
    }
}