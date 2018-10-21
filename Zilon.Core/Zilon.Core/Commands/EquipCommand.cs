using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Client;
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
            var equipment = GetEquipment();
            if (equipment == null)
            {
                return false;
            }

            if (SlotIndex == null)
            {
                throw new InvalidOperationException("Для команды не указан слот.");
            }

            var equipmentCarrier = _playerState.ActiveActor.Actor.Person.EquipmentCarrier;
            var slot = equipmentCarrier.Slots[SlotIndex.Value];
            if ((slot.Types & equipment.Scheme.Equip.SlotTypes[0]) == 0)
            {
                return false;
            }

            return true;
        }

        private Equipment GetEquipment()
        {
            var propVm = _inventoryState.SelectedProp;
            var equipment = propVm?.Prop as Equipment;

            return equipment;
        }

        protected override void ExecuteTacticCommand()
        {
            if (SlotIndex == null)
            {
                throw new InvalidOperationException("Для команды не указан слот.");
            }

            var propVm = _inventoryState.SelectedProp;
            var equipment = propVm.Prop as Equipment;
            if (equipment == null)
            {
                throw new InvalidOperationException("Попытка экипировать то, что не является экипировкой.");
            }

            var intention = new Intention<EquipTask>(a => new EquipTask(a, equipment, SlotIndex.Value));
            _playerState.TaskSource.Intent(intention);
        }
    }
}