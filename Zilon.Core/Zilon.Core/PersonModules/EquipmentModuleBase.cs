namespace Zilon.Core.PersonModules
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Components;

    using Persons;

    using Props;

    using Schemes;

    /// <summary>
    /// Базовая реализация моделя работы с экипировкой.
    /// </summary>
    /// <seealso cref="Zilon.Core.Persons.IEquipmentCarrier" />
    public abstract class EquipmentModuleBase : IEquipmentModule
    {
        private readonly Equipment?[] _equipment;

        /// <summary>
        /// Конструирует экземпляр модуля работы с экипировкой типа <see cref="EquipmentCarrierBase" />.
        /// </summary>
        /// <param name="equipments">Стартовая экипировка.</param>
        protected EquipmentModuleBase(IEnumerable<Equipment?> equipments)
        {
            _equipment = equipments.ToArray();
        }

        /// <summary>
        /// Конструирует экземпляр модуля работы с экипировкой типа <see cref="EquipmentCarrierBase" />.
        /// </summary>
        /// <param name="size">Количество элементов экипировки.</param>
        protected EquipmentModuleBase(int size)
        {
            _equipment = new Equipment?[size];
        }

        /// <summary>
        /// Конструирует экземпляр модуля работы с экипировкой типа <see cref="EquipmentCarrierBase" />.
        /// </summary>
        /// <param name="slots">Набор слотов, на основе которого создаётся модель работы с экипировкой.</param>
        protected EquipmentModuleBase([NotNull] IReadOnlyCollection<PersonSlotSubScheme> slots)
        {
            if (slots == null)
            {
                throw new ArgumentNullException(nameof(slots));
            }

            var slotArray = slots as PersonSlotSubScheme[] ?? slots.ToArray();
            if (!slotArray.Any())
            {
                throw new ArgumentException("Коллекция слотов не может быть пустой.");
            }

            Slots = slotArray;

            _equipment = new Equipment?[Slots.Length];

            IsActive = true;
        }

        /// <summary>
        /// Выбрасывает событие <see cref="EquipmentChanged" /> с указыннми данными в аргументах.
        /// </summary>
        /// <param name="slotIndex">Индекс слота, в котором произошли изменения.</param>
        /// <param name="oldEquipment">Старая экипировка, которая была до изменнеия слота.</param>
        /// <param name="equipment">Текущая экипировка.</param>
        protected virtual void DoEquipmentChanged(int slotIndex,
            Equipment? oldEquipment,
            Equipment? equipment)
        {
            EquipmentChanged?.Invoke(
                sender: this,
                e: new EquipmentChangedEventArgs(equipment: equipment, oldEquipment: oldEquipment,
                    slotIndex: slotIndex));
        }

        /// <summary>
        /// Проверяет возможность установки предмета в указанный слот экипировки.
        /// Используется в this[].set.
        /// </summary>
        /// <param name="equipment">Предмет, который будет экипирован.</param>
        /// <param name="slotIndex">Слот, в который будет произведена экипировка.</param>
        /// <remarks>
        /// При нарушении условий будет выбрасывать исключение.
        /// </remarks>
        protected abstract void ValidateSetEquipment(Equipment equipment, int slotIndex);

        private void DropHandsEquipment(IEnumerable<int> foundHandsIndexes)
        {
            foreach (var handIndex in foundHandsIndexes)
            {
                _equipment[handIndex] = null;
            }
        }

        private IEnumerable<int> FoundHandsIndexes()
        {
            return Enumerable.Range(start: 0, count: _equipment.Length).Where(
                i =>
                {
                    var slotByIndex = Slots[i];
                    var isHand = slotByIndex?.Types == EquipmentSlotTypes.Hand;
                    return isHand;
                });
        }

        private void ReplaceEquipmentInHandSlots(Equipment? equipment)
        {
            var foundHandsIndexes = FoundHandsIndexes();
            if (!foundHandsIndexes.Any())
            {
                throw new ArgumentException($"No hand slots to equipment the {equipment}");
            }

            DropHandsEquipment(foundHandsIndexes);
            var firstHandIndex = foundHandsIndexes.First();
            _equipment[firstHandIndex] = equipment;
        }

        private void SetEquipment(Equipment? equipment, int slotIndex)
        {
            if (equipment != null)
            {
                ValidateSetEquipment(equipment: equipment, slotIndex: slotIndex);

                var isTwoHandedEquipment = equipment?.Scheme?.Equip?.EquipRestrictions?.PropHandUsage ==
                                           PropHandUsage.TwoHanded;
                if (isTwoHandedEquipment)
                {
                    ReplaceEquipmentInHandSlots(equipment);
                }
                else
                {
                    _equipment[slotIndex] = equipment;
                }
            }
            else
            {
                _equipment[slotIndex] = null;
            }

            var oldEquipment = _equipment[slotIndex];

            DoEquipmentChanged(slotIndex: slotIndex, oldEquipment: oldEquipment, equipment: equipment);
        }

        /// <summary>
        /// Возвращает энумератор, который перебирает текущую экипировку.
        /// </summary>
        /// <returns>
        /// Энумератор, который может быть использован для перебора текущей экипировки.
        /// </returns>
        public IEnumerator<Equipment?> GetEnumerator()
        {
            var enumerable = _equipment.AsEnumerable();
            return enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _equipment.GetEnumerator();
        }

        /// <summary>
        /// Экипировка персонажа.
        /// </summary>
        /// <remarks>
        /// Если указан экземпляр предмета, то производится попытка установки предмета
        /// с учётом правил экипировки (двуручное занимает две руки, пистолеты только в единственном экземпляре).
        /// При необходимости, согласно релизации этого интерфейса, предметы, несовместимые с устанавливаемым,
        /// могут изыматся из слотов в инвентарь или уничтожаться.
        /// Если указано null, то экипировка изымается из указанного слота.
        /// </remarks>
        [MaybeNull]
        public virtual Equipment? this[int index]
        {
            get => _equipment[index];
            set => SetEquipment(equipment: value, slotIndex: index);
        }

        /// <summary>
        /// Текущие слоты экипировки.
        /// </summary>
        public abstract PersonSlotSubScheme[] Slots { get; protected set; }

        /// <inheritdoc />
        public string Key => nameof(IEquipmentModule);

        /// <inheritdoc />
        public bool IsActive { get; set; }

        /// <summary>
        /// Выстреливает, когда экипировка изменяется.
        /// </summary>
        public event EventHandler<EquipmentChangedEventArgs>? EquipmentChanged;
    }
}