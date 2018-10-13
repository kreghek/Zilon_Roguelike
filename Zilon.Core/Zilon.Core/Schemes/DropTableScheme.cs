using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема тарблицы дропа.
    /// </summary>
    public sealed class DropTableScheme : SchemeBase, IDropTableScheme
    {
        /// <summary>
        /// Записи в таблице дропа.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public DropTableRecordSubScheme[] Records { get; }

        /// <summary>
        /// Количество бросков на проверку выпавшей записи.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public int Rolls { get; }

        [ExcludeFromCodeCoverage]
        public DropTableScheme(int rolls, params DropTableRecordSubScheme[] records)
        {
            Rolls = rolls;
            Records = records;
        }
    }
}
