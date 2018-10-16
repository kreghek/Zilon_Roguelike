using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Components;
using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    public class EquipmentCarrier : IEquipmentCarrier
    {
        public EquipmentCarrier(IEnumerable<PersonSlotSubScheme> slots)
        {
            Slots = slots.ToArray();

            Equipments = new Equipment[Slots.Count()];
        }

        public Equipment[] Equipments { get; }

        public PersonSlotSubScheme[] Slots { get; }

        public event EventHandler<EquipmentChangedEventArgs> EquipmentChanged;


        public void SetEquipment(Equipment equipment, int slotIndex)
        {
            var oldEquipment = Equipments[slotIndex];

            if (equipment != null)
            {
                var slot = Slots[slotIndex];

                if ((slot.Types & equipment.Scheme.Equip.SlotTypes[0]) > 0)
                {
                    Equipments[slotIndex] = equipment;
                }
                else
                {
                    throw new ArgumentException($"Для экипировки указан слот {slot}, не подходящий для данного типа предмета {equipment}.");
                }
            }
            else
            {
                Equipments[slotIndex] = equipment;
            }
            

            DoEquipmentChanged(slotIndex, oldEquipment, equipment);
        }

        private int FindFreeSlotIndex(EquipmentSlotTypes requiredSlotTypes)
        {
            for (var i = 0; i < Slots.Count(); i++)
            {
                var slot = Slots[i];

                if (Equipments[i] != null)
                {
                    continue;
                }

                if ((slot.Types & requiredSlotTypes) > 0)
                {
                    return i;
                }
            }

            return -1;
        }

        private void DoEquipmentChanged(int slotIndex,
            Equipment oldEquipment,
            Equipment equipment)
        {
            EquipmentChanged?.Invoke(this, new EquipmentChangedEventArgs(equipment, oldEquipment, slotIndex));
        }
    }
}
