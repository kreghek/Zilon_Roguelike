using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Bot.Players.Triggers
{
    public sealed class HasBetterEquipmentTrigger : ILogicStateTrigger
    {
        public void Reset()
        {
            // Нет состояния.
        }

        public bool Test(IActor actor, ILogicState currentState, ILogicStrategyData strategyData)
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
