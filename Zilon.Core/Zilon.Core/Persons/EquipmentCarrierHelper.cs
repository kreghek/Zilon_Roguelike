using System.Linq;

using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    public static class EquipmentCarrierHelper
    {
        public static bool CheckSlotCompability(Equipment equipment, PersonSlotSubScheme slot)
        {
            var invalidSlot = (slot.Types & equipment.Scheme.Equip.SlotTypes[0]) == 0;
            if (invalidSlot)
            {
                return false;
            }

            return true;
        }

        public static bool CheckDualCompability(IEquipmentCarrier equipmentCarrier, Equipment equipment, PersonSlotSubScheme slot, int slotIndex)
        {
            var equipmentTags = equipment.Scheme.Tags ?? new string[0];
            var hasRangedTag = equipmentTags.Any(x => x == PropTags.Equipment.Ranged);
            var hasWeaponTag = equipmentTags.Any(x => x == PropTags.Equipment.Weapon);
            if (hasRangedTag && hasWeaponTag)
            {
                // Проверяем наличие любого экипированного оружия.
                // Если находим, то выбрасываем исключение.
                // Учитываем, что предмет в целевом слоте заменяется.
                var targetSlotEquipment = equipmentCarrier.Equipments[slotIndex];
                var currentEquipments = equipmentCarrier.Equipments.Where(x => x != null);
                var currentWeapons = from currentEquipment in currentEquipments
                                     where currentEquipment != targetSlotEquipment
                                     let currentEqupmentTags = currentEquipment.Scheme.Tags ?? new string[0]
                                     where currentEqupmentTags.Any(x => x == PropTags.Equipment.Weapon)
                                     select currentEquipment;

                var hasWeapon = currentWeapons.Any();

                if (hasWeapon)
                {
                    return false;
                }
            }

            if (hasWeaponTag)
            {
                // проверяем наличие стрелкового оружия.
                // Если находим, то выбрасываем исключение.
                // Учитываем, что предмет в целевом слоте заменяется.
                var targetSlotEquipment = equipmentCarrier.Equipments[slotIndex];
                var currentEquipments = equipmentCarrier.Equipments.Where(x => x != null);
                var currentWeapons = from currentEquipment in currentEquipments
                                     where currentEquipment != targetSlotEquipment
                                     let currentEqupmentTags = currentEquipment.Scheme.Tags ?? new string[0]
                                     let currentEqupmentHasWeapon = currentEqupmentTags.Any(x => x == PropTags.Equipment.Weapon)
                                     let currentEqupmentHasRanged = currentEqupmentTags.Any(x => x == PropTags.Equipment.Ranged)
                                     where currentEqupmentHasWeapon && currentEqupmentHasRanged
                                     select currentEquipment;

                var hasWeapon = currentWeapons.Any();

                if (hasWeapon)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool CheckSheildCompability(IEquipmentCarrier equipmentCarrier, Equipment equipment, PersonSlotSubScheme slot, int slotIndex)
        {
            var equipmentTags = equipment.Scheme.Tags ?? new string[0];

            var hasShieldTag = equipmentTags.Any(x => x == PropTags.Equipment.Shield);
            if (hasShieldTag)
            {
                // Проверяем наличие других щитов.
                // Если в другой руке щит уже экипирован, то выбрасываем исключение.
                // Учитываем, что предмет в целевом слоте заменяется.
                var targetSlotEquipment = equipmentCarrier.Equipments[slotIndex];
                var currentEquipments = equipmentCarrier.Equipments.Where(x => x != null);
                var currentSheilds = from currentEquipment in currentEquipments
                                     where currentEquipment != targetSlotEquipment
                                     let currentEqupmentTags = currentEquipment.Scheme.Tags ?? new string[0]
                                     where currentEqupmentTags.Any(x => x == PropTags.Equipment.Shield)
                                     select currentEquipment;

                var hasSheidls = currentSheilds.Any();

                if (hasSheidls)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
