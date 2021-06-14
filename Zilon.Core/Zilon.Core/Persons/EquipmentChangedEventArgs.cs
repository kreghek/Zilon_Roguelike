using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Props;

namespace Zilon.Core.Persons
{
    public class EquipmentChangedEventArgs : EventArgs
    {
        [ExcludeFromCodeCoverage]
        public EquipmentChangedEventArgs(Equipment? equipment,
            Equipment? oldEquipment,
            int slotIndex)
        {
            Equipment = equipment;
            OldEquipment = oldEquipment;
            SlotIndex = slotIndex;
        }

        public Equipment? Equipment { get; }

        public Equipment? OldEquipment { get; }

        public int SlotIndex { get; }
    }
}