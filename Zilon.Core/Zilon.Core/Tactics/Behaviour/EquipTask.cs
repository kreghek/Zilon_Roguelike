using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Zilon.Core.Components;
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

        private void EquipOneSlotEquipmentFromInventory(
            IEquipmentModule equipmentModule,
            Equipment targetEquipment,
            Equipment? currentEquipment)
        {
            if (currentEquipment != null)
            {
                Actor.Person.GetModule<IInventoryModule>().Add(currentEquipment);
            }

            Actor.Person.GetModule<IInventoryModule>().Remove(targetEquipment);
            equipmentModule[_slotIndex] = targetEquipment;
        }

        private void EquipPropToSlot()
        {
            var equipmentCarrier = Actor.Person.GetModule<IEquipmentModule>();

            // Предмет может быть экипирован из инвентаря (и) или из другого слота (с).
            // Предмет может быть экипирован в пустой слот (0) и слот, в котором уже есть другой предмет (1).
            //    (и)                                           (с)
            // (0) изымаем предмет из инвентаря                 меняем предметы в слотах местами
            // (1) изымаем из инвентаря, а текущий в инвентярь  меняем предметы в слотах местами
            //TODO Ещё в этой схеме учитывает однослотовые и двуслотовые предметы.

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

                        EquipOneSlotEquipmentFromInventory(equipmentCarrier, _equipment, currentEquipment);
                    }
                    else if (equipRestrictions.PropHandUsage.GetValueOrDefault().HasFlag(PropHandUsage.TwoHanded))
                    {
                        // Equip two handed weapon/tool in 2 of all slots.

                        EquipTwoSlotEquipmentFromInventory(equipmentCarrier, _equipment, currentEquipment);
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

                    EquipOneSlotEquipmentFromInventory(equipmentCarrier, _equipment, currentEquipment);
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

        private void EquipTwoSlotEquipmentFromInventory(
            IEquipmentModule equipmentModule,
            Equipment targetEquipment,
            Equipment? currentEquipment)
        {
            var handSlotIndexes = GetHandSlotIndexes(equipmentModule);
            var otherHandSlotIndex = handSlotIndexes.First(x => x != _slotIndex);

            var equipmentInHandSlots = new List<Equipment>();
            var equipmentInTargetSlot = equipmentModule[_slotIndex];
            Debug.Assert(equipmentInTargetSlot == currentEquipment, "This is same equipment.");
            if (equipmentInTargetSlot is not null)
            {
                equipmentInHandSlots.Add(equipmentInTargetSlot);
            }

            var equipmentInOtherSlot = equipmentModule[otherHandSlotIndex];
            if (equipmentInOtherSlot is not null)
            {
                equipmentInHandSlots.Add(equipmentInOtherSlot);
            }

            foreach (var equipment in equipmentInHandSlots)
            {
                Actor.Person.GetModule<IInventoryModule>().Add(equipment);
            }

            Actor.Person.GetModule<IInventoryModule>().Remove(targetEquipment);
            equipmentModule[_slotIndex] = targetEquipment;
            equipmentModule[otherHandSlotIndex] = null;
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

        private static IEnumerable<int> GetHandSlotIndexes(IEquipmentModule equipmentModule)
        {
            for (var slotIndex = 0; slotIndex < equipmentModule.Slots.Length; slotIndex++)
            {
                if (equipmentModule.Slots[slotIndex].Types.HasFlag(EquipmentSlotTypes.Hand))
                {
                    yield return slotIndex;
                }
            }
        }

        private static bool IsTargetSlotBeHand(IEquipmentModule equipmentModule, int slotIndex)
        {
            if (!equipmentModule.Slots.Any())
            {
                // Module has no slots.
                // So it is not hand slot.
                // Used for tests.
                return false;
            }

            return equipmentModule.Slots[slotIndex].Types.HasFlag(EquipmentSlotTypes.Hand);
        }
    }
}