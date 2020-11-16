using System.Linq;

using Zilon.Core.Common;
using Zilon.Core.PersonModules;
using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    public static class EquipmentCarrierHelper
    {
        public static bool CanBeEquiped(IEquipmentModule equipmentCarrier, int slotIndex, Equipment equipment)
        {
            if (equipmentCarrier is null)
            {
                throw new System.ArgumentNullException(nameof(equipmentCarrier));
            }

            if (equipment is null)
            {
                throw new System.ArgumentNullException(nameof(equipment));
            }

            var slot = equipmentCarrier.Slots[slotIndex];

            if (!CheckSlotCompability(equipment, slot))
            {
                return false;
            }

            if (!CheckDualCompability(equipmentCarrier, equipment, slotIndex))
            {
                return false;
            }

            if (!CheckShieldCompability(equipmentCarrier, equipment, slotIndex))
            {
                return false;
            }

            return true;
        }

        public static bool CheckDualCompability(IEquipmentModule equipmentModule, Equipment equipment, int slotIndex)
        {
            if (equipmentModule is null)
            {
                throw new System.ArgumentNullException(nameof(equipmentModule));
            }

            if (equipment is null)
            {
                throw new System.ArgumentNullException(nameof(equipment));
            }

            var equipmentTags = equipment.Scheme.Tags ?? System.Array.Empty<string>();
            var hasRangedTag = equipmentTags.Any(x => x == PropTags.Equipment.Ranged);
            var hasWeaponTag = equipmentTags.Any(x => x == PropTags.Equipment.Weapon);
            if (hasRangedTag && hasWeaponTag)
            {
                // Проверяем наличие любого экипированного оружия.
                // Если находим, то выбрасываем исключение.
                // Учитываем, что предмет в целевом слоте заменяется.
                var targetSlotEquipment = equipmentModule[slotIndex];
                var currentEquipments = equipmentModule.Where(x => x != null);
                var currentWeapons = from currentEquipment in currentEquipments
                    where currentEquipment != targetSlotEquipment
                    let currentEqupmentTags = currentEquipment.Scheme.Tags ?? System.Array.Empty<string>()
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
                var targetSlotEquipment = equipmentModule[slotIndex];
                var currentEquipments = equipmentModule.Where(x => x != null);
                var currentWeapons = from currentEquipment in currentEquipments
                    where currentEquipment != targetSlotEquipment
                    let currentEqupmentTags = currentEquipment.Scheme.Tags ?? System.Array.Empty<string>()
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

        public static bool CheckShieldCompability(IEquipmentModule equipmentCarrier, Equipment equipment, int slotIndex)
        {
            if (equipmentCarrier is null)
            {
                throw new System.ArgumentNullException(nameof(equipmentCarrier));
            }

            if (equipment is null)
            {
                throw new System.ArgumentNullException(nameof(equipment));
            }

            var equipmentTags = equipment.Scheme.Tags ?? System.Array.Empty<string>();

            var hasShieldTag = equipmentTags.Any(x => x == PropTags.Equipment.Shield);
            if (hasShieldTag)
            {
                // Проверяем наличие других щитов.
                // Если в другой руке щит уже экипирован, то выбрасываем исключение.
                // Учитываем, что предмет в целевом слоте заменяется.
                var targetSlotEquipment = equipmentCarrier[slotIndex];
                var currentEquipments = equipmentCarrier.Where(x => x != null);
                var currentSheilds = from currentEquipment in currentEquipments
                    where currentEquipment != targetSlotEquipment
                    let currentEqupmentTags = currentEquipment.Scheme.Tags ?? System.Array.Empty<string>()
                    where currentEqupmentTags.Any(x => x == PropTags.Equipment.Shield)
                    select currentEquipment;

                var hasShields = currentSheilds.Any();
                if (hasShields)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool CheckSlotCompability(Equipment equipment, PersonSlotSubScheme slot)
        {
            if (equipment is null)
            {
                throw new System.ArgumentNullException(nameof(equipment));
            }

            if (slot is null)
            {
                throw new System.ArgumentNullException(nameof(slot));
            }

            var invalidSlot = (slot.Types & equipment.Scheme.Equip.SlotTypes[0]) == 0;
            if (invalidSlot)
            {
                return false;
            }

            return true;
        }
    }
}