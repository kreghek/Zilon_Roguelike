using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    public class EquipmentCarrier : IEquipmentCarrier
    {
        public EquipmentCarrier([NotNull] [ItemNotNull] IEnumerable<PersonSlotSubScheme> slots)
        {
            if (slots == null)
            {
                throw new ArgumentNullException(nameof(slots));
            }

            if (slots.Count() == 0)
            {
                throw new ArgumentException("Коллекция слотов не может быть пустой.");
            }

            Slots = slots.ToArray();

            Equipments = new Equipment[Slots.Length];
        }

        public Equipment this[int index]
        {
            get
            {
                return Equipments[index];
            }
            set
            {
                SetEquipment(value, index);
            }
        }

        private Equipment[] Equipments { get; }

        public PersonSlotSubScheme[] Slots { get; }

        public event EventHandler<EquipmentChangedEventArgs> EquipmentChanged;


        private void SetEquipment(Equipment equipment, int slotIndex)
        {
            var oldEquipment = Equipments[slotIndex];

            if (equipment != null)
            {
                var slot = Slots[slotIndex];

                if (!EquipmentCarrierHelper.CheckSlotCompability(equipment, slot))
                {
                    throw new ArgumentException($"Для экипировки указан слот {slot}, не подходящий для данного типа предмета {equipment}.");
                }

                if (!EquipmentCarrierHelper.CheckDualCompability(this, equipment, slot, slotIndex))
                {
                    throw new InvalidOperationException($"Попытка экипировать предмет {equipment}, несовместимый с текущий экипировкой.");
                }

                if (!EquipmentCarrierHelper.CheckSheildCompability(this, equipment, slot, slotIndex))
                {
                    throw new InvalidOperationException("Попытка экипировать два щита.");
                }

                Equipments[slotIndex] = equipment;
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

        public IEnumerator<Equipment> GetEnumerator()
        {
            return Equipments.Cast<Equipment>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Equipments.GetEnumerator();
        }
    }
}
