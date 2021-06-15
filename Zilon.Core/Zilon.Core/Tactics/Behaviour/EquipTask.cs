using System;
using System.Diagnostics;
using System.Linq;

using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Задача на назначение экипировки в указанный слот.
    /// </summary>
    public class EquipTask : OneTurnActorTaskBase
    {
        private readonly Equipment? _equipment;
        private readonly int _slotIndex;

        public EquipTask(IActor actor,
            IActorTaskContext context,
            Equipment? equipment,
            int slotIndex) :
            base(actor, context)
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
            if (_equipment is null)
            {
                // There is no reason to check equipemnt is null.
                // But analyser can't understand condition in parent method ExecuteTask.
                throw new InvalidOperationException();
            }

            var currentEquipedSlotIndex = FindPropInEquiped(_equipment, equipmentCarrier);

            var equipScheme = _equipment.Scheme.Equip;
            if (equipScheme is null)
            {
                throw new InvalidOperationException($"{_equipment.Scheme.Sid} is not equipment.");
            }

            if (currentEquipedSlotIndex is null)
            {
                // (и)

                // текущий предмет возвращаем в инвентарь (1)
                // при (0) ничего не делаем

                if (IsTargetSlotBeHand(equipmentCarrier, _slotIndex))
                {
                    // Follow to logic of one/two hand equipment
                    var equipRestrictions = equipScheme.EquipRestrictions;
                    if (equipRestrictions is null || equipRestrictions.PropHandUsage is null)
                    {
                        // Equip one-handed item in a specified hand.

                        EquipOneSlotEquipment(equipmentCarrier, _equipment, currentEquipment);
                    }
                    else if (equipRestrictions.PropHandUsage.GetValueOrDefault().HasFlag(PropHandUsage.TwoHanded))
                    {
                        // Equip two handed weapon/tool in 2 of all slots.

                        //TODO this must be rewriten to meet issue requirements.
                        EquipOneSlotEquipment(equipmentCarrier, _equipment, currentEquipment);
                    }
                    else
                    {
                        //Do nothings beacause this state is unknown.
                        Debug.Fail("Unknown state");
                    }
                }
                else
                {
                    // Follow to logic of other slots.

                    EquipOneSlotEquipment(equipmentCarrier, _equipment, currentEquipment);
                }
            }
            else
            {
                // (с)

                if (currentEquipment != null)
                {
                    if (IsTargetSlotBeHand(equipmentCarrier, _slotIndex))
                    {
                        var equipRestrictions = equipScheme.EquipRestrictions;
                        if (equipRestrictions is null || equipRestrictions.PropHandUsage is null)
                        {
                            // (1) Ставим существующий в данном слоте предмет в слот, в котором был выбранный предмет
                            equipmentCarrier[currentEquipedSlotIndex.Value] = currentEquipment;
                        }
                        else if (equipRestrictions.PropHandUsage.GetValueOrDefault().HasFlag(PropHandUsage.TwoHanded))
                        {
                            //TODO this must be rewriten to meet issue requirements.
                            // Considering a two-handed item must takes 2 hand slots.
                            // So target item can take 2 slots, no hand slots will be free, current equipment must be place in inventory.

                            // (1) Ставим существующий в данном слоте предмет в слот, в котором был выбранный предмет
                            equipmentCarrier[currentEquipedSlotIndex.Value] = currentEquipment;
                        }
                    }
                    else
                    {
                        // (1) Ставим существующий в данном слоте предмет в слот, в котором был выбранный предмет
                        equipmentCarrier[currentEquipedSlotIndex.Value] = currentEquipment;
                    }
                    
                }
                else
                {
                    if (IsTargetSlotBeHand(equipmentCarrier, _slotIndex))
                    {
                        var equipRestrictions = equipScheme.EquipRestrictions;
                        if (equipRestrictions is null || equipRestrictions.PropHandUsage is null)
                        {
                            // В старый слот выбранного предмета записываем пустоту.
                            // Потому что предмет перенесён из этого слота в другой.
                            equipmentCarrier[currentEquipedSlotIndex.Value] = null;
                        }
                        else if (equipRestrictions.PropHandUsage.GetValueOrDefault().HasFlag(PropHandUsage.TwoHanded))
                        {
                            //TODO this must be rewriten to meet issue requirements.
                            // Considering a two-handed item must takes 2 hand slots.
                            // So target item can take 2 slots, no hand slots will be free, current equipment must be place in inventory.

                            // В старый слот выбранного предмета записываем пустоту.
                            // Потому что предмет перенесён из этого слота в другой.
                            equipmentCarrier[currentEquipedSlotIndex.Value] = null;
                        }
                    }
                    else
                    {
                        // В старый слот выбранного предмета записываем пустоту.
                        // Потому что предмет перенесён из этого слота в другой.
                        equipmentCarrier[currentEquipedSlotIndex.Value] = null;
                    }
                }

                equipmentCarrier[_slotIndex] = _equipment;
            }
        }

        private static bool IsTargetSlotBeHand(IEquipmentModule equipmentCarrier, int slotIndex)
        {
            return equipmentCarrier.Slots[slotIndex].Types.HasFlag(Components.EquipmentSlotTypes.Hand);
        }

        private void EquipOneSlotEquipment(IEquipmentModule equipmentCarrier, Equipment targetEquipment, Equipment? currentEquipment)
        {
            if (currentEquipment != null)
            {
                Actor.Person.GetModule<IInventoryModule>().Add(currentEquipment);
            }

            Actor.Person.GetModule<IInventoryModule>().Remove(targetEquipment);
            equipmentCarrier[_slotIndex] = targetEquipment;
        }

        /// <summary>
        /// Ищем предмет в уже экипированных.
        /// </summary>
        /// <param name="equipment"> Целевой предмет. </param>
        /// <param name="equipmentModule"> Объект для хранения экипировки. </param>
        /// <returns> Возвращает индекс слота, в котором находится указанный предмет. Или null, если предмет не найден. </returns>
        private static int? FindPropInEquiped(Equipment equipment, IEquipmentModule equipmentModule)
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