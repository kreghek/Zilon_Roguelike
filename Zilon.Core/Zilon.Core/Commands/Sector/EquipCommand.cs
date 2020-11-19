using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Client;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Команда на назначение экипировки.
    /// </summary>
    public class EquipCommand : SpecialActorCommandBase
    {
        private readonly IInventoryState _inventoryState;
        private readonly IPlayer _player;

        [ExcludeFromCodeCoverage]
        public EquipCommand(
            IPlayer player,
            ISectorUiState playerState,
            IInventoryState inventoryState) :
            base(playerState)
        {
            _player = player;
            _inventoryState = inventoryState;
        }

        public int? SlotIndex { get; set; }

        public override bool CanExecute()
        {
            if (_inventoryState.SelectedProp == null)
            {
                return true;
            }

            var equipment = GetInventorySelectedEquipment();
            if (equipment is null && _inventoryState.SelectedProp != null)
            {
                return false;
            }

            // Сломанную экипировку нельзя надевать
            //TODO Тут есть замечание, что equipment не проверяется.
            // Реорганизовать этот код в более понятный.
            if (equipment != null && equipment.Durable.Value <= 0)
            {
                return false;
            }

            if (SlotIndex == null)
            {
                throw new InvalidOperationException("Для команды не указан слот.");
            }

            var equipmentCarrier = PlayerState.ActiveActor.Actor.Person.GetModule<IEquipmentModule>();
            var slot = equipmentCarrier.Slots[SlotIndex.Value];

            var canEquipInSlot = EquipmentCarrierHelper.CheckSlotCompability(equipment, slot);
            if (!canEquipInSlot)
            {
                return false;
            }

            var canEquipDual = EquipmentCarrierHelper.CheckDualCompability(equipmentCarrier,
                equipment,
                SlotIndex.Value);
            if (!canEquipDual)
            {
                return false;
            }

            var canEquipShield = EquipmentCarrierHelper.CheckShieldCompability(equipmentCarrier,
                equipment,
                SlotIndex.Value);

            if (!canEquipShield)
            {
                return false;
            }

            return true;
        }

        protected override void ExecuteTacticCommand()
        {
            if (SlotIndex == null)
            {
                throw new InvalidOperationException("Для команды не указан слот.");
            }

            var equipment = GetInventorySelectedEquipment();

            var taskContext = new ActorTaskContext(_player.SectorNode.Sector);

            var intention = new Intention<EquipTask>(a => new EquipTask(a, taskContext, equipment, SlotIndex.Value));
            PlayerState.TaskSource.Intent(intention, PlayerState.ActiveActor.Actor);
        }

        private Equipment GetInventorySelectedEquipment()
        {
            var propVieModel = _inventoryState.SelectedProp;
            if (propVieModel == null)
            {
                return null;
            }

            if (propVieModel.Prop is Equipment equipment)
            {
                return equipment;
            }

            return null;
        }
    }
}