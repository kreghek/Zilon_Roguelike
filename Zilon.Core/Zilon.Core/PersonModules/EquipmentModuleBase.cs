using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.PersonModules
{
    /// <summary>
    /// Базовая реализация моделя работы с экипировкой.
    /// </summary>
    /// <seealso cref="Zilon.Core.Persons.IEquipmentCarrier" />
    public abstract class EquipmentModuleBase : IEquipmentModule
    {
        private readonly Equipment[] _equipment;

        /// <summary>
        /// Конструирует экземпляр модуля работы с экипировкой типа <see cref="EquipmentCarrierBase"/>.
        /// </summary>
        /// <param name="equipments">Стартовая экипировка.</param>
        protected EquipmentModuleBase(IEnumerable<Equipment> equipments)
        {
            _equipment = equipments.ToArray();
        }

        /// <summary>
        /// Конструирует экземпляр модуля работы с экипировкой типа <see cref="EquipmentCarrierBase"/>.
        /// </summary>
        /// <param name="size">Количество элементов экипировки.</param>
        protected EquipmentModuleBase(int size)
        {
            _equipment = new Equipment[size];
        }

        /// <summary>
        /// Конструирует экземпляр модуля работы с экипировкой типа <see cref="EquipmentCarrierBase"/>.
        /// </summary>
        /// <param name="slots">Набор слотов, на основе которого создаётся модель работы с экипировкой.</param>
        protected EquipmentModuleBase([NotNull][ItemNotNull] IEnumerable<PersonSlotSubScheme> slots)
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

            _equipment = new Equipment[Slots.Length];

            IsActive = true;
        }

        /// <summary>
        /// Возвращает энумератор, который перебирает текущую экипировку.
        /// </summary>
        /// <returns>
        /// Энумератор, который может быть использован для перебора текущей экипировки.
        /// </returns>
        public IEnumerator<Equipment> GetEnumerator()
        {
            return _equipment.AsEnumerable().GetEnumerator();
        }

        /// <summary>
        /// Выбрасывает событие <see cref="EquipmentChanged"/> с указыннми данными в аргументах.
        /// </summary>
        /// <param name="slotIndex">Индекс слота, в котором произошли изменения.</param>
        /// <param name="oldEquipment">Старая экипировка, которая была до изменнеия слота.</param>
        /// <param name="equipment">Текущая экипировка.</param>
        protected virtual void DoEquipmentChanged(
            int slotIndex,
            Equipment oldEquipment,
            Equipment equipment)
        {
            EquipmentChanged?.Invoke(this, new EquipmentChangedEventArgs(equipment, oldEquipment, slotIndex));
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _equipment.GetEnumerator();
        }

        private void SetEquipment(Equipment equipment, int slotIndex)
        {
            if (equipment != null)
            {
                ValidateSetEquipment(equipment, slotIndex);

                _equipment[slotIndex] = equipment;
            }
            else
            {
                _equipment[slotIndex] = null;
            }

            var oldEquipment = _equipment[slotIndex];

            DoEquipmentChanged(slotIndex, oldEquipment, equipment);
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
        [CanBeNull]
        public virtual Equipment this[int index]
        {
            get => _equipment[index];
            set => SetEquipment(value, index);
        }

        /// <summary>
        /// Текущие слоты экипировки.
        /// </summary>
        public abstract PersonSlotSubScheme[] Slots { get; protected set; }

        /// <inheritdoc/>
        public string Key => nameof(IEquipmentModule);

        /// <inheritdoc/>
        public bool IsActive { get; set; }

        /// <summary>
        /// Выстреливает, когда экипировка изменяется.
        /// </summary>
        public event EventHandler<EquipmentChangedEventArgs> EquipmentChanged;
    }
}