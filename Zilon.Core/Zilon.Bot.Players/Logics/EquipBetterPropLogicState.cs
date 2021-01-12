using System;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Logics
{
    public sealed class EquipBetterPropLogicState : LogicStateBase
    {
        public override IActorTask GetTask(IActor actor, ISectorTaskSourceContext context,
            ILogicStrategyData strategyData)
        {
            if (strategyData.TargetEquipment is null || strategyData.TargetEquipmentSlot is null)
            {
                throw new InvalidOperationException("Assign TargetEquipment and TargetEquipmentSlot in trigger first.");
            }

            var targetEquipmentFromInventory = strategyData.TargetEquipment;
            var targetSlotIndex = strategyData.TargetEquipmentSlot;

            var taskContext = new ActorTaskContext(context.Sector);

            var task = new EquipTask(actor, taskContext, targetEquipmentFromInventory, targetSlotIndex.Value);
            Complete = true;
            return task;
        }

        protected override void ResetData()
        {
            // Нет состояния.
        }
    }
}