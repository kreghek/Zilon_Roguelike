using System;

namespace Zilon.Core.Persons
{
    public class EquipmentChangedEventArgs : EventArgs
    {
        public Equipment Equipment { get; }
        public Equipment OldEquipment { get; }
        public int SlotIndex { get; }

        public EquipmentChangedEventArgs(Equipment equipment,
            Equipment oldEquipment,
            int slotIndex)
        {
            Equipment = equipment;
            OldEquipment = oldEquipment;
            SlotIndex = slotIndex;
        }
    }
}
