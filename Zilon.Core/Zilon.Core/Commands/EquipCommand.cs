using System;

using Zilon.Core.Client;
using Zilon.Core.Persons;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Команда на назначение экипировки.
    /// </summary>
    public class EquipCommand : SpecialActorCommandBase
    {
        private readonly IInventoryState _inventoryState;

        public int? SlotIndex { get; set; }

        public EquipCommand(ISectorManager sectorManager,
            IPlayerState playerState,
            IInventoryState inventoryState) :
            base(sectorManager, playerState)
        {
            _inventoryState = inventoryState;
        }

        public override bool CanExecute()
        {
            if (SlotIndex == null)
            {
                throw new InvalidOperationException("Для команды не указан слот.");
            }

            var propVm = _inventoryState.SelectedProp;
            if (propVm == null)
            {
                return false;
            }

            var equipment = propVm.Prop as Equipment;

            if (equipment == null)
            {
                return false;
            }

            //TODO Добавить проверку на то, что выбранный предмет может быть экипирован в указанный слот.

            return true;
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

            //_playerState.TaskSource.IntentEquip(equipment, SlotIndex.Value);
            _playerState.TaskSource.Intent(null);
        }
    }
}