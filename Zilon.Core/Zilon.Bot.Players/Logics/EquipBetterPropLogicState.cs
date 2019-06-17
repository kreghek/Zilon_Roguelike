using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Logics
{
    public sealed class EquipBetterPropLogicState : LogicStateBase
    {
        public override IActorTask GetTask(IActor actor, ILogicStrategyData strategyData)
        {
            var currentInventoryEquipments = actor.Person.Inventory.CalcActualItems().OfType<Equipment>();
            var emptyEquipmentSlots = new List<PersonSlotSubScheme>();

            for (int i = 0; i < actor.Person.EquipmentCarrier.Slots.Length; i++)
            {
                var slot = actor.Person.EquipmentCarrier.Slots[i];
                var equiped = actor.Person.EquipmentCarrier[i];
                if (equiped == null)
                {
                    var availableEquipments = currentInventoryEquipments
                        .Where(x => (x.Scheme.Equip.SlotTypes[0] & slot.Types) > 0);

                    if (availableEquipments.Any())
                    {
                        var targetEquipmentFromInventory = availableEquipments.First();
                        var targetSlotIndex = i;

                        return new EquipTask(actor, targetEquipmentFromInventory, targetSlotIndex);
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
