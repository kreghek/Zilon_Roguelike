using System;

using Zilon.Core.PersonModules;
using Zilon.Core.Persons;

namespace Zilon.Core.Props
{
    /// <summary>
    /// Реализация сервиса для работы с прочностью экипировки.
    /// </summary>
    /// <seealso cref="IEquipmentDurableService" />
    public sealed class EquipmentDurableService : IEquipmentDurableService
    {
        private const int SUCCESS_TURN_RESIST = 2;
        private const int SUCCESS_USE_RESIST = 6;
        private const float FIELD_REDUCE_MAX = 0.1f;

        private readonly IEquipmentDurableServiceRandomSource _randomSource;

        /// <summary>
        /// Создаёт экземпляр <see cref="EquipmentDurableService"/>.
        /// </summary>
        /// <param name="randomSource">Источник рандома для сервиса.</param>
        public EquipmentDurableService(IEquipmentDurableServiceRandomSource randomSource)
        {
            _randomSource = randomSource;
        }

        private static void UnequipIfDurableIsOver(Equipment equipment, IPerson equipmentOwner)
        {
            if (equipment.Durable.Value <= 0)
            {
                var equipmentCarrier = equipmentOwner.GetModule<IEquipmentModule>();
                if (equipmentCarrier == null)
                {
                    return;
                }

                int? slotIndex = null;
                for (var i = 0; i < equipmentCarrier.Slots.Length; i++)
                {
                    var prop = equipmentCarrier[i];
                    if (prop == equipment)
                    {
                        slotIndex = i;
                        break;
                    }
                }

                if (slotIndex != null)
                {
                    equipmentOwner.UnequipProp(slotIndex.Value);
                }
            }
        }

        /// <summary>Определяет, может ли экипировка быть отремонтирована.</summary>
        /// <param name="equipment">Целевая экипировка.</param>
        /// <returns>
        ///   <c>true</c> если ремонт возможет; Иначе, <c>false</c> (не подлежит восстановлению, на утилизацию).
        /// </returns>
        public bool CanBeRepaired(Equipment equipment)
        {
            if (equipment is null)
            {
                throw new ArgumentNullException(nameof(equipment));
            }

            return equipment.Durable.Range.Max > 1;
        }

        /// <summary>Восстановление прочности экипировки.</summary>
        /// <param name="repairResource">Ресурс, при помощи которого производится ремонт.</param>
        /// <param name="equipment">Целевая экипировка.</param>
        /// <exception cref="ArgumentException">Указанная экипировка не может быть отремонтирована. - equipment</exception>
        public void Repair(IProp repairResource, Equipment equipment)
        {
            if (!CanBeRepaired(equipment))
            {
                throw new ArgumentException("Указанная экипировка не может быть отремонтирована.", nameof(equipment));
            }

            var maxDurable = equipment.Durable.Range.Max;
            var reduceValue = (int)Math.Round(maxDurable * FIELD_REDUCE_MAX) + 1;

            equipment.Durable.ChangeStatRange(0, maxDurable - reduceValue);
        }

        /// <summary>Обновляет прочность экипировки со временем.</summary>
        /// <param name="equipment">Целевая экипировка.</param>
        public void UpdateByTurn(Equipment equipment, IPerson equipmentOwner)
        {
            if (equipment is null)
            {
                throw new ArgumentNullException(nameof(equipment));
            }

            var resistRoll = _randomSource.RollTurnResist(equipment);
            if (resistRoll < SUCCESS_TURN_RESIST)
            {
                equipment.Durable.Value--;
            }

            UnequipIfDurableIsOver(equipment, equipmentOwner);
        }

        /// <summary>Обновляет прочность экипировки при использовании.</summary>
        /// <param name="equipment">Целевая экипировка.</param>
        public void UpdateByUse(Equipment equipment, IPerson equipmentOwner)
        {
            if (equipment is null)
            {
                throw new ArgumentNullException(nameof(equipment));
            }

            var resistRoll = _randomSource.RollUseResist(equipment);
            if (resistRoll < SUCCESS_USE_RESIST)
            {
                equipment.Durable.Value--;
            }

            UnequipIfDurableIsOver(equipment, equipmentOwner);
        }
    }
}