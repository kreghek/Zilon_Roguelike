using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Client;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
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

            if (_inventoryState.SelectedProp is null)
            {
                return new CanExecuteCheckResult
                {
                    IsSuccess = false,
                    FailureReason = "Item to equip is not selected."
                };
            }

            if (_inventoryState.SelectedProp.Prop is not Equipment equipmentFromInventory)
            {
                return new CanExecuteCheckResult
                {
                    IsSuccess = false,
                    FailureReason = "It is attempt to equip non-equipment."
                };
            }

            // Сломанную экипировку нельзя надевать
            if (equipmentFromInventory.Durable.Value <= 0)
            {
                return new CanExecuteCheckResult
                {
                    IsSuccess = false,
                    FailureReason = "The selected equipment is broken."
                };
            }

            var equipmentCarrier = PlayerState.ActiveActor!.Actor.Person.GetModule<IEquipmentModule>();
            var slot = equipmentCarrier.Slots[SlotIndex.Value];

            var canEquipInSlot = EquipmentCarrierHelper.CheckSlotCompability(equipmentFromInventory!, slot);
            if (!canEquipInSlot)
            {
                return new CanExecuteCheckResult
                {
                    IsSuccess = false,
                    FailureReason = $"Incompatible slot {slot?.Types} to assign equipment."
                };
            }

            var canEquipDual = EquipmentCarrierHelper.CheckDualCompability(equipmentCarrier,
                equipmentFromInventory,
                SlotIndex.Value);
            if (!canEquipDual)
            {
                return new CanExecuteCheckResult
                {
                    IsSuccess = false,
                    FailureReason = "Equipment is not compatible to dual."
                };
            }

            var canEquipShield = EquipmentCarrierHelper.CheckShieldCompability(equipmentCarrier,
                equipmentFromInventory,
                SlotIndex.Value);

            if (!canEquipShield)
            {
                return new CanExecuteCheckResult
                {
                    IsSuccess = false,
                    FailureReason = "It is attempt to equip second shield."
                };
            }

            var is1hTo2hSlot = Check2hOnlyInMainHandSlot(equipmentCarrier,
                equipmentFromInventory,
                SlotIndex.Value);
            if (is1hTo2hSlot != null && is1hTo2hSlot == false)
            {
                return new CanExecuteCheckResult
                {
                    IsSuccess = false,
                    FailureReason = "It is attempt to equip two-handed in not main hand slot."
                };
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

        private static bool? Check2hOnlyInMainHandSlot(IEquipmentModule equipmentModule, Equipment targetItemToEquip,
            int slotIndex)
        {
            var equipRestrictions = targetItemToEquip.Scheme?.Equip?.EquipRestrictions;
            if (equipRestrictions is null || equipRestrictions.PropHandUsage is null)
            {
                // Equiped item is one-handed or not in hand slot.
                // No special rules of can execute checking are need.
                return true;
            }

            if (equipRestrictions.PropHandUsage == PropHandUsage.TwoHanded)
            {
                return equipmentModule.Slots[slotIndex].IsMain;
            }

            throw new InvalidOperationException("Unknown case.");
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