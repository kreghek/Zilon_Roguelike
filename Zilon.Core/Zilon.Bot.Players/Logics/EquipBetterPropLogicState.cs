using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Logics
{
    public sealed class EquipBetterPropLogicState : LogicStateBase
    {
        public override IActorTask GetTask(
            IActor actor,
            ISectorTaskSourceContext context,
            ILogicStrategyData strategyData)
        {
            var inventory = actor.Person.GetModule<IInventoryModule>();
            var currentInventoryProps = inventory.CalcActualItems();
            var currentInventoryEquipments = currentInventoryProps.OfType<Equipment>().ToArray();

            var equipmentCarrier = actor.Person.GetModule<IEquipmentModule>();

            for (var slotIndex = 0; slotIndex < equipmentCarrier.Slots.Length; slotIndex++)
            {
                var slot = actor.Person.GetModule<IEquipmentModule>().Slots[slotIndex];
                var equiped = actor.Person.GetModule<IEquipmentModule>()[slotIndex];
                if (equiped == null)
                {
                    var availableEquipments = currentInventoryEquipments
                        .Where(equipment => (equipment.Scheme.Equip.SlotTypes[0] & slot.Types) > 0)
                        .Where(equipment => EquipmentCarrierHelper.CanBeEquiped(equipmentCarrier, slotIndex, equipment))
                        .ToArray();

                    if (availableEquipments.Any())
                    {
                        var targetEquipmentFromInventory = availableEquipments.First();
                        var targetSlotIndex = slotIndex;

                        var taskContext = new ActorTaskContext(context.Sector);
                        return new EquipTask(actor, taskContext, targetEquipmentFromInventory, targetSlotIndex);
                    }
                }
            }

            Complete = true;
            return null;
        }

        protected override void ResetData()
        {
            // Нет состояния.
        }
    }
}