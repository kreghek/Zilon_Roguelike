using System.Collections.Generic;

using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Сервис для генерации трофеев по таблицам дропа.
    /// </summary>
    public interface IDropResolver
    {
        /// <summary>
        /// Полчение предметов по указанным таблицам дропа.
        /// </summary>
        /// <param name="dropTables"> Таблицы дропа. </param>
        /// <returns> Возвращает сгенерированные готовые предметы. </returns>
        IProp[] GetProps(IEnumerable<IDropTableScheme> dropTables);
    }
}
