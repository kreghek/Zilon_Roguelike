using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Client;
using Zilon.Core.Persons;
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

        public int? SlotIndex { get; set; }

        [ExcludeFromCodeCoverage]
        public EquipCommand(IGameLoop gameLoop,
            ISectorManager sectorManager,
            IPlayerState playerState,
            IInventoryState inventoryState) :
            base(gameLoop, sectorManager, playerState)
        {
            _inventoryState = inventoryState;
        }

        public override bool CanExecute()
        {
            if (_inventoryState.SelectedProp == null)
            {
                return true;
            }

            var equipment = GetEquipment();
            if (equipment == null && _inventoryState.SelectedProp != null)
            {
                return false;
            }

            if (SlotIndex == null)
            {
                throw new InvalidOperationException("Для команды не указан слот.");
            }

            var equipmentCarrier = PlayerState.ActiveActor.Actor.Person.EquipmentCarrier;
            var slot = equipmentCarrier.Slots[SlotIndex.Value];

            var canEquipInSlot = EquipmentCarrierHelper.CheckSlotCompability(equipment, slot);
            if (!canEquipInSlot)
            {
                return false;
            }

            var canEquipDual = EquipmentCarrierHelper.CheckDualCompability(equipmentCarrier,
                equipment,
                slot,
                SlotIndex.Value);
            if (!canEquipDual)
            {
                return false;
            }

            var canEquipShield = EquipmentCarrierHelper.CheckSheildCompability(equipmentCarrier,
                equipment,
                slot,
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

            var equipment = GetEquipment();

            var intention = new Intention<EquipTask>(a => new EquipTask(a, equipment, SlotIndex.Value));
            PlayerState.TaskSource.Intent(intention);

        }

        private Equipment GetEquipment()
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