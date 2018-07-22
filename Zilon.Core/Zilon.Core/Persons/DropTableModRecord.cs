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
        /// Запись таблицы дропа.
        /// </summary>
        public DropTableRecordSubScheme Record { get; set; }

        /// <summary>
        /// Модифицированный вес.
        /// </summary>
        public int ModifiedWeight { get; set; }
    }
}
