using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.PersonModules
{
    public class EquipmentModule : EquipmentModuleBase
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        // This value was assigned in base class.
        public EquipmentModule(IReadOnlyCollection<PersonSlotSubScheme> slots)
            : base(slots)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        public override PersonSlotSubScheme[] Slots { get; protected set; }

        protected override void ValidateSetEquipment(Equipment equipment, int slotIndex)
        {
            var slot = GetSlotForEquipByIndex(equipment, slotIndex);

            if (!EquipmentCarrierHelper.CheckSlotCompability(equipment, slot))
            {
                throw new ArgumentException(
                    $"Для экипировки указан слот {slot}, не подходящий для данного типа предмета {equipment}.");
            }

            if (!EquipmentCarrierHelper.CheckDualCompability(
                equipmentModule: this,
                equipment,
                slotIndex))
            {
                throw new InvalidOperationException(
                    $"Попытка экипировать предмет {equipment}, несовместимый с текущий экипировкой.");
            }

            if (!EquipmentCarrierHelper.CheckShieldCompability(
                equipmentCarrier: this,
                equipment,
                slotIndex))
            {
                throw new InvalidOperationException("Попытка экипировать два щита.");
            }
        }

        private PersonSlotSubScheme GetSlotForEquipByIndex(Equipment equipment, int slotIndex)
        {
            var personSlotSubScheme = Slots.ElementAtOrDefault(slotIndex);
            return personSlotSubScheme ?? throw new ArgumentException(
                $"Attempting to equip an item {equipment} in a nonexistent {slotIndex} slot");
        }
    }
}