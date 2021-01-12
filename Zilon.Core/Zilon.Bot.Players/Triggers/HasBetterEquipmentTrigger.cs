using System.Linq;

using Zilon.Core.Components;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Triggers
{
    public sealed class HasBetterEquipmentTrigger : ILogicStateTrigger
    {
        private static bool IsApplicableForSlot(Equipment equipment, EquipmentSlotTypes slotTypes)
        {
            return (equipment.Scheme.Equip.SlotTypes[0] & slotTypes) > 0;
        }

        public void Reset()
        {
            // Нет состояния.
        }

        public bool Test(IActor actor, ISectorTaskSourceContext context, ILogicState currentState,
            ILogicStrategyData strategyData)
        {
            if (actor is null)
            {
                throw new System.ArgumentNullException(nameof(actor));
            }

            if (currentState is null)
            {
                throw new System.ArgumentNullException(nameof(currentState));
            }

            if (strategyData is null)
            {
                throw new System.ArgumentNullException(nameof(strategyData));
            }

            if (!actor.Person.HasModule<IInventoryModule>() || !actor.Person.HasModule<IEquipmentModule>())
            {
                return false;
            }

            var inventoryModule = actor.Person.GetModule<IInventoryModule>();
            var inventoryProps = inventoryModule.CalcActualItems();
            var currentInventoryEquipments = inventoryProps.OfType<Equipment>();
            if (!currentInventoryEquipments.Any())
            {
                strategyData.TargetEquipment = null;
                strategyData.TargetEquipmentSlot = null;
                return false;
            }

            var equipmentModule = actor.Person.GetModule<IEquipmentModule>();
            for (var slotIndex = 0; slotIndex < equipmentModule.Slots.Length; slotIndex++)
            {
                var slotScheme = equipmentModule.Slots[slotIndex];
                var equiped = equipmentModule[slotIndex];
                if (equiped is null)
                {
                    var availableEquipments = from equipment in currentInventoryEquipments
                                              where IsApplicableForSlot(equipment, slotScheme.Types)
                                              where EquipmentCarrierHelper.CanBeEquiped(equipmentModule, slotIndex,
                                                  equipment)
                                              select equipment;

                    var targetEquipmentFromInventory = availableEquipments.FirstOrDefault();
                    if (targetEquipmentFromInventory != null)
                    {
                        strategyData.TargetEquipment = targetEquipmentFromInventory;
                        strategyData.TargetEquipmentSlot = slotIndex;
                        return true;
                    }
                }
            }

            return false;
        }

        public void Update()
        {
            // Нет состояния.
        }
    }
}