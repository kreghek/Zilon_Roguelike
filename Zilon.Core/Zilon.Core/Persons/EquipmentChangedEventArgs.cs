using System;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

using Zilon.Core.Props;

namespace Zilon.Core.Persons
{
    public class EquipmentChangedEventArgs : EventArgs
    {
        [PublicAPI]
        public Equipment Equipment { get; }

        [PublicAPI]
        public Equipment OldEquipment { get; }

        [PublicAPI]
        public int SlotIndex { get; }

        [ExcludeFromCodeCoverage]
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