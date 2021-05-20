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

        public override CanExecuteCheckResult CanExecute()
        {
            if (_inventoryState.SelectedProp is null)
            {
                // Means equipment will be unequiped from the slot (specified in command).
                return CanExecuteCheckResult.CreateSuccessful();
            }

            if (SlotIndex is null)
            {
                throw new InvalidOperationException("Для команды не указан слот.");
            }

            var equipmentFromInventory = GetSelectedEquipmentInInventory();
            if (equipmentFromInventory is null && _inventoryState.SelectedProp != null)
            {
                return new CanExecuteCheckResult { IsSuccess = false };
            }

            // Сломанную экипировку нельзя надевать
            //TODO Тут есть замечание, что equipment не проверяется.
            // Реорганизовать этот код в более понятный.
            if (equipmentFromInventory != null && equipmentFromInventory.Durable.Value <= 0)
            {
                return new CanExecuteCheckResult { IsSuccess = false };
            }

            var equipmentCarrier = PlayerState.ActiveActor!.Actor.Person.GetModule<IEquipmentModule>();
            var slot = equipmentCarrier.Slots[SlotIndex.Value];

            var canEquipInSlot = EquipmentCarrierHelper.CheckSlotCompability(equipmentFromInventory!, slot);
            if (!canEquipInSlot)
            {
                return new CanExecuteCheckResult { IsSuccess = false };
            }

            var canEquipDual = EquipmentCarrierHelper.CheckDualCompability(equipmentCarrier,
                equipmentFromInventory!,
                SlotIndex.Value);
            if (!canEquipDual)
            {
                return new CanExecuteCheckResult { IsSuccess = false };
            }

            var canEquipShield = EquipmentCarrierHelper.CheckShieldCompability(equipmentCarrier,
                equipmentFromInventory!,
                SlotIndex.Value);

            if (!canEquipShield)
            {
                return new CanExecuteCheckResult { IsSuccess = false };
            }

            return new CanExecuteCheckResult { IsSuccess = true };
        }

        protected override void ExecuteTacticCommand()
        {
            if (SlotIndex is null)
            {
                throw new InvalidOperationException("Для команды не указан слот.");
            }

            var equipment = GetSelectedEquipmentInInventory();

            var sector = _player.SectorNode.Sector;
            if (sector is null)
            {
                throw new InvalidOperationException();
            }

            var taskContext = new ActorTaskContext(sector);

            var intention = new Intention<EquipTask>(a => new EquipTask(a, taskContext, equipment, SlotIndex.Value));
            var taskSource = PlayerState.TaskSource;
            if (taskSource is null)
            {
                throw new InvalidOperationException();
            }

            var activeActor = PlayerState.ActiveActor?.Actor;
            if (activeActor is null)
            {
                throw new InvalidOperationException();
            }

            taskSource.Intent(intention, activeActor);
        }

        private Equipment? GetSelectedEquipmentInInventory()
        {
            var propVieModel = _inventoryState.SelectedProp;
            if (propVieModel is null)
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