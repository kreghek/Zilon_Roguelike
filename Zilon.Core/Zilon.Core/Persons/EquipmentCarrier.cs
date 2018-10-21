using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    public class EquipmentCarrier : IEquipmentCarrier
    {
        public EquipmentCarrier(IEnumerable<PersonSlotSubScheme> slots)
        {
            Slots = slots.ToArray();

            Equipments = new Equipment[Slots.Length];
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
                Equipments[slotIndex] = null;
            }
            

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
