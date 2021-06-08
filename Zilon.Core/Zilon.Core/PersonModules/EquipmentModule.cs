namespace Zilon.Core.PersonModules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Persons;

    using Props;

    using Schemes;

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
            var slot = GetSlotForEquipByIndex(equipment: equipment, slotIndex: slotIndex);

            if (!EquipmentCarrierHelper.CheckSlotCompability(equipment: equipment, slot: slot))
            {
                throw new ArgumentException(
                    $"Для экипировки указан слот {slot}, не подходящий для данного типа предмета {equipment}.");
            }

            if (!EquipmentCarrierHelper.CheckDualCompability(equipmentModule: this, equipment: equipment,
                slotIndex: slotIndex))
            {
                throw new InvalidOperationException(
                    $"Попытка экипировать предмет {equipment}, несовместимый с текущий экипировкой.");
            }

            if (!EquipmentCarrierHelper.CheckShieldCompability(
                equipmentCarrier: this,
                equipment: equipment,
                slotIndex: slotIndex))
            {
                throw new InvalidOperationException("Попытка экипировать два щита.");
            }
        }

        private PersonSlotSubScheme GetSlotForEquipByIndex(Equipment equipment, int slotIndex)
        {
            return Slots.ElementAtOrDefault(slotIndex) ?? throw new ArgumentException(
                $"Попытка экипировать предмет {equipment} в несуществующий слот {slotIndex}");
        }
    }
}