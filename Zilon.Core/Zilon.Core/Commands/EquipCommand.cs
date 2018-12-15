using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Zilon.Core.Client;
using Zilon.Core.Persons;
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

            var equipmentCarrier = PlayerState.ActiveActor.Actor.Person.EquipmentCarrier;
            var canEquipInSlot = CheckSlot(equipmentCarrier, equipment);
            if (!canEquipInSlot)
            {
                return false;
            }

            var canEquipDual = CheckDual(equipmentCarrier, equipment, SlotIndex.Value);
            if (!canEquipDual)
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
            if (equipment == null)
            {
                throw new InvalidOperationException("Попытка экипировать то, что не является экипировкой.");
            }

            var intention = new Intention<EquipTask>(a => new EquipTask(a, equipment, SlotIndex.Value));
            PlayerState.TaskSource.Intent(intention);

        }

        private Equipment GetEquipment()
        {
            var propVm = _inventoryState.SelectedProp;
            var equipment = propVm?.Prop as Equipment;

            return equipment;
        }

        private bool CheckSlot(IEquipmentCarrier equipmentCarrier, Equipment equipment)
        {
            var slot = equipmentCarrier.Slots[SlotIndex.Value];
            if ((slot.Types & equipment.Scheme.Equip.SlotTypes[0]) == 0)
            {
                return false;
            }

            return true;
        }

        private bool CheckDual(IEquipmentCarrier equipmentCarrier, Equipment equipment, int slotIndex)
        {
            var equipmentTags = equipment.Scheme.Tags ?? new string[0];
            var hasRangedTag = equipmentTags.Any(x => x == PropTags.Equipment.Ranged);
            var hasWeaponTag = equipmentTags.Any(x => x == PropTags.Equipment.Weapon);
            if (hasRangedTag && hasWeaponTag)
            {
                // Проверяем наличие любого экипированного оружия.
                // Если находим, то выбрасываем исключение.
                var targetSlotEquipment = equipmentCarrier.Equipments[slotIndex];
                var currentEquipments = equipmentCarrier.Equipments.Where(x => x != null);
                var currentWeapons = from currentEquipment in currentEquipments
                                     where currentEquipment != targetSlotEquipment
                                     let currentEqupmentTags = currentEquipment.Scheme.Tags ?? new string[0]
                                     where currentEqupmentTags.Any(x => x == PropTags.Equipment.Weapon)
                                     select currentEquipment;

                var hasWeapon = currentWeapons.Any();

                if (hasWeapon)
                {
                    return false;
                }
            }

            return true;
        }
    }
}