using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    public class EquipmentCarrier : IEquipmentCarrier
    {
        public EquipmentCarrier(IEnumerable<PersonSlotSubScheme> slots)
        {
            Slots = slots;

            Equipments = new Equipment[Slots.Count()];
        }

        public Equipment[] Equipments { get; }

        public IEnumerable<PersonSlotSubScheme> Slots { get; }

        public event EventHandler<EquipmentChangedEventArgs> EquipmentChanged;


        public void SetEquipment(Equipment equipment, int slotIndex)
        {
            var oldEquipment = Equipments[slotIndex];
            Equipments[slotIndex] = equipment;

            DoEquipmentChanged(slotIndex, oldEquipment, equipment);
        }

        private void DoEquipmentChanged(int slotIndex,
            Equipment oldEquipment,
            Equipment equipment)
        {
            EquipmentChanged?.Invoke(this, new EquipmentChangedEventArgs(equipment, oldEquipment, slotIndex));
        }
    }
}
