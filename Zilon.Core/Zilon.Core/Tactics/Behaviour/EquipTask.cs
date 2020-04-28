using System.Linq;
using Zilon.Core.PersonModules;
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
            // Первоначально процедура экипировки предмета в слот инвентаря
            // делится на перемещение предмета в слот и обнуление слота.
            if (_equipment != null)
            {
                EquipPropToSlot();
            }
            else
            {
                Actor.Person.UnequipProp(_slotIndex);
            }
        }

        private void EquipPropToSlot()
        {
            var equipmentCarrier = Actor.Person.GetModule<IEquipmentModule>();

            // Предмет может быть экипирован из инвентаря (и) или из другого слота (с).
            // Предмет может быть экипирован в пустой слот (0) и слот, в котором уже есть другой предмет (1).
            //    (и)                                           (с)
            // (0) изымаем предмет из инвентаря                 меняем предметы в слотах местами
            // (1) изымаем из инвентаря, а текущий в инвентярь  меняем предметы в слотах местами


            // проверяем, есть ли в текущем слоте предмет (0)/(1).
            var currentEquipment = equipmentCarrier[_slotIndex];

            // проверяем, из инвентаря или из слота экипируем (и)/(с)
            var currentEquipedSlotIndex = FindPropInEquiped(_equipment, equipmentCarrier);

            if (currentEquipedSlotIndex == null)
            {
                // (и)

                // текущий предмет возвращаем в инвентарь (1)
                // при (0) ничего не делаем
                if (currentEquipment != null)
                {
                    Actor.Person.Inventory.Add(currentEquipment);
                }

                Actor.Person.Inventory.Remove(_equipment);
                equipmentCarrier[_slotIndex] = _equipment;
            }
            else
            {
                // (с)

                if (currentEquipment != null)
                {
                    // (1) Ставим существующий в данном слоте предмет предмет в слот, в котором был выбранный предмет
                    equipmentCarrier[currentEquipedSlotIndex.Value] = currentEquipment;
                }
                else
                {
                    // В старый слот выбранного предмета записываем пустоту.
                    // Потому что предмет перенесён из этого слота в другой.
                    equipmentCarrier[currentEquipedSlotIndex.Value] = null;
                }

                equipmentCarrier[_slotIndex] = _equipment;
            }
        }

        /// <summary>
        /// Ищем предмет в уже экипированных.
        /// </summary>
        /// <param name="equipment"> Целевой предмет. </param>
        /// <param name="equipmentModule"> Объект для хранения экипировки. </param>
        /// <returns> Возвращает индекс слота, в котором находится указанный предмет. Или null, если предмет не найден. </returns>
        private int? FindPropInEquiped(Equipment equipment, IEquipmentModule equipmentModule)
        {
            for (var i = 0; i < equipmentModule.Count(); i++)
            {
                if (equipmentModule[i] == equipment)
                {
                    return i;
                }
            }

            return null;
        }
    }
}
