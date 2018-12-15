using System;
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

        public Equipment[] Equipments { get; }

        public PersonSlotSubScheme[] Slots { get; }

        public event EventHandler<EquipmentChangedEventArgs> EquipmentChanged;


        public void SetEquipment(Equipment equipment, int slotIndex)
        {
            var oldEquipment = Equipments[slotIndex];

            if (equipment != null)
            {
                var slot = Slots[slotIndex];

                CheckSlotCompability(equipment, slot);
                CheckDualCompability(equipment, slot);

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

        private static void CheckSlotCompability(Equipment equipment, PersonSlotSubScheme slot)
        {
            var invalidSlot = (slot.Types & equipment.Scheme.Equip.SlotTypes[0]) == 0;
            if (invalidSlot)
            {
                throw new ArgumentException($"Для экипировки указан слот {slot}, не подходящий для данного типа предмета {equipment}.");
            }
        }

        private void CheckDualCompability(Equipment equipment, PersonSlotSubScheme slot)
        {
            var equipmentTags = equipment.Scheme.Tags ?? new string[0];
            var hasRangedTag = equipmentTags.Any(x => x == PropTags.Equipment.Ranged);
            var hasWeaponTag = equipmentTags.Any(x => x == PropTags.Equipment.Weapon);
            if (hasRangedTag && hasWeaponTag)
            {
                // Проверяем наличие любого экипированного оружия.
                // Если находим, то выбрасываем исключение.
                var currentEquipments = Equipments.Where(x => x != null);
                var currentWeapons = from currentEquipment in currentEquipments
                                     let currentEqupmentTags = currentEquipment.Scheme.Tags ?? new string[0]
                                     where currentEqupmentTags.Any(x => x == PropTags.Equipment.Weapon)
                                     select currentEquipment;

                var hasWeapon = currentWeapons.Any();

                if (hasWeapon)
                {
                    throw new InvalidOperationException("Попытка экипировать два стрелковых оружия.");
                }
            }
        }
    }
}
