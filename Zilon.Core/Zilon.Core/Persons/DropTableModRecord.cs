using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Вспомогательный класс для записи таблицы дропа с учётом модифицированного веса.
    /// </summary>
    /// <remarks>
    /// Введён для того, чтобы не изменять иммутабельную схему записи таблицы дропа.
    /// </remarks>
    public sealed class DropTableModRecord
    {
        /// <summary>
        /// Модифицированный вес.
        /// </summary>
        public int ModifiedWeight { get; set; }

        /// <summary>
        /// Запись таблицы дропа.
        /// </summary>
        public IDropTableRecordSubScheme Record { get; set; }
    }
}