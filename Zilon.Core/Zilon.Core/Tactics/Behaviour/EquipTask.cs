using Zilon.Core.Persons;

namespace Zilon.Core.Tactics.Behaviour
{
    //TODO объединить все одноходовые задачи
    /// <summary>
    /// Задача на назначение экипировки в указанный слот.
    /// </summary>
    public class EquipTask : ActorTaskBase
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

        public override void Execute()
        {
            Actor.Person.EquipmentCarrier.SetEquipment(_equipment, _slotIndex);
            IsComplete = true;
        }
    }
}
