using Zilon.Core.Persons;
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

            // Предмет может быть экипирован из инвентаря (и) или из другого слота (с).
            // Предмет может быть экипирован в пустой слот (0) и слот, в котором уже есть другой предмет (1).
            //    (и)                                           (с)
            // (0) изымаем предмет из инвентаря                 меняем предметы в слотах местами
            // (1) изымаем из инвентаря, а текущий в инвентярь  меняем предметы в слотах местами


            // проверяем, есть ли в текущем слоте предмет (0)/(1).
            var currentEquipment = equipmentCarrier.Equipments[_slotIndex];

            // проверяем, из инвентаря или из слота экипируем (и)/(с)
            var currentEquipedSlotIndex = FindPropInEquiped(_equipment, equipmentCarrier);

            if (currentEquipedSlotIndex == null)
            {
                // (и)

                // текущий предмет возвращаем в инвентарь (1)
                // про (0) ничего не делаем
                if (currentEquipment != null)
                {
                    Actor.Person.Inventory.Add(currentEquipment);
                }

                Actor.Person.Inventory.Remove(_equipment);
                equipmentCarrier.SetEquipment(_equipment, _slotIndex);
            }
            else
            {
                // (с)

                if (currentEquipment != null)
                {
                    // (1) Ставим существующий в данном слоте предмет предмет в слот, в котором был выбранный предмет
                    equipmentCarrier.SetEquipment(currentEquipment, currentEquipedSlotIndex.Value);
                }
                else
                {
                    // В старый слот выбранного предмета записываем пустоту.
                    // Потому что предмет перенесён из этого слота в другой.
                    equipmentCarrier.SetEquipment(null, currentEquipedSlotIndex.Value);
                }

                equipmentCarrier.SetEquipment(_equipment, _slotIndex);
            }            
        }

        /// <summary>
        /// Ищем предмет в уже экипированных.
        /// </summary>
        /// <param name="equipmentCarrier"></param>
        /// <returns></returns>
        private int? FindPropInEquiped(Equipment equipment, IEquipmentCarrier equipmentCarrier)
        {
            for (var i = 0; i < equipmentCarrier.Equipments.Length; i++)
            {
                if (equipmentCarrier.Equipments[i] == equipment)
                {
                    return i;
                }
            }

            return null;
        }
    }
}
