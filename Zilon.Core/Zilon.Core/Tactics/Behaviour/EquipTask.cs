using Zilon.Core.Props;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Задача на назначение экипировки в указанный слот.
    /// </summary>
    public class EquipTask : OneTurnActorTaskBase
    {
        private readonly Equipment _equipment;
        private readonly int _slotIndex;

        public EquipTask(IActor actor,
            Equipment equipment,
            int slotIndex) :
            base(actor)
        {
            _equipment = equipment;
            _slotIndex = slotIndex;
        }

        protected override void ExecuteTask()
        {
            var equipmentCarrier = Actor.Person.EquipmentCarrier;

            var currentEquipment = equipmentCarrier.Equipments[_slotIndex];
            if (currentEquipment != null)
            {
                // Означает, что предмет экипироуется из инвентаря.
                Actor.Person.Inventory.Add(currentEquipment);
            }
            else
            {
                int? currentEquipedSlotIndex = null;
                for (var i = 0; i < equipmentCarrier.Equipments.Length; i++)
                {
                    if (equipmentCarrier.Equipments[i] == _equipment)
                    {
                        currentEquipedSlotIndex = i;
                    }
                }

                if (currentEquipedSlotIndex != null)
                {
                    // Означает, что предмет был экипирован в другой слой и мы его перекладываем
                    equipmentCarrier.Equipments[currentEquipedSlotIndex.Value] = null;
                }

            }

            equipmentCarrier.SetEquipment(_equipment, _slotIndex);

            Actor.Person.Inventory.Remove(_equipment);
        }
    }
}
