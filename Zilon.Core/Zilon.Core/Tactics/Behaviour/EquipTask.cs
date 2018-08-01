using Zilon.Core.Persons;

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
                Actor.Person.Inventory.Add(currentEquipment);
            }

            equipmentCarrier.SetEquipment(_equipment, _slotIndex);

            Actor.Person.Inventory.Remove(_equipment);
        }
    }
}
