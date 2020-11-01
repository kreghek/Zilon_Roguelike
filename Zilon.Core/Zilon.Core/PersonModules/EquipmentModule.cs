using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.PersonModules
{
    public class EquipmentModule : EquipmentModuleBase
    {
        public EquipmentModule([NotNull] [ItemNotNull] IEnumerable<PersonSlotSubScheme> slots) : base(slots)
        {
        }

        public override PersonSlotSubScheme[] Slots { get; protected set; }

        protected override void ValidateSetEquipment(Equipment equipment, int slotIndex)
        {
            PersonSlotSubScheme slot = Slots[slotIndex];

            if (!EquipmentCarrierHelper.CheckSlotCompability(equipment, slot))
            {
                throw new ArgumentException(
                    $"Для экипировки указан слот {slot}, не подходящий для данного типа предмета {equipment}.");
            }

            if (!EquipmentCarrierHelper.CheckDualCompability(this, equipment, slotIndex))
            {
                throw new InvalidOperationException(
                    $"Попытка экипировать предмет {equipment}, несовместимый с текущий экипировкой.");
            }

            if (!EquipmentCarrierHelper.CheckShieldCompability(this, equipment, slotIndex))
            {
                throw new InvalidOperationException("Попытка экипировать два щита.");
            }
        }
    }
}