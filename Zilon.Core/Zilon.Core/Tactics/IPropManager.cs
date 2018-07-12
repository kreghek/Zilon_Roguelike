using System.Collections.Generic;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Интерфейс менеджера предметов.
    /// </summary>
    public interface IPropManager
    {
        /// <summary>
        /// Текущий список всех предметов в секторе.
        /// </summary>
        IEnumerable<ISectorProp> Props { get; }

        /// <summary>
        /// Добавляет предмет в общий список.
        /// </summary>
        /// <param name="prop"> Целевой предмет. </param>
        void Add(ISectorProp prop);

        /// <summary>
        /// Добавляет несколько предметов в общий список.
        /// </summary>
        /// <param name="props"> Перечень предметов. </param>
        void Add(IEnumerable<ISectorProp> props);
    }
}
