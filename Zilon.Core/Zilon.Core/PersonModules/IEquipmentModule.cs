using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.PersonModules
{
    public interface IEquipmentModule : IPersonModule, IEnumerable<Equipment>
    {
        /// <summary>
        /// Текущие слоты экипировки.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        PersonSlotSubScheme[] Slots { get; }

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
        Equipment this[int index] { get; set; }

        /// <summary>
        /// Выстреливает, когда экипировка изменяется.
        /// </summary>
        event EventHandler<EquipmentChangedEventArgs> EquipmentChanged;
    }
}