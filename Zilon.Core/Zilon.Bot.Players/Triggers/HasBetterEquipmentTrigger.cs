using System.Linq;

using Zilon.Core.PersonModules;
using Zilon.Core.Props;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Triggers
{
    public sealed class HasBetterEquipmentTrigger : ILogicStateTrigger
    {
        public void Reset()
        {
            // Нет состояния.
        }

        public bool Test(IActor actor, ISectorTaskSourceContext context, ILogicState currentState, ILogicStrategyData strategyData)
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

            var currentInventoryEquipments = actor.Person.GetModule<IInventoryModule>().CalcActualItems().OfType<Equipment>();

            for (int i = 0; i < actor.Person.GetModule<IEquipmentModule>().Slots.Length; i++)
            {
                var slot = actor.Person.GetModule<IEquipmentModule>().Slots[i];
                var equiped = actor.Person.GetModule<IEquipmentModule>()[i];
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
