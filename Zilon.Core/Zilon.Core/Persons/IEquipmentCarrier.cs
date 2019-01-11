using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Интерфейс для работы с экипировкой.
    /// </summary>
    public interface IEquipmentCarrier: IEnumerable<Equipment>
    {
        [NotNull] [ItemNotNull] PersonSlotSubScheme[] Slots { get; }

        /// <summary>
        /// Выстреливает, когда экипировка изменяется.
        /// </summary>
        event EventHandler<EquipmentChangedEventArgs> EquipmentChanged;

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
        [NotNull, ItemCanBeNull]
        Equipment this[int index] { get; set; }
    }
}
