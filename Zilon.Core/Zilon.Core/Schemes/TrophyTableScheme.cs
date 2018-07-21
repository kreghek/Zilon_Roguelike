namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема тарблицы дропа.
    /// </summary>
    public sealed class TrophyTableScheme: SchemeBase
    {
        /// <summary>
        /// Записи в таблице дропа.
        /// </summary>
        public TrophyTableRecordSubScheme[] Records { get; set; }

        /// <summary>
        /// Количество бросков на проверку выпавшей записи.
        /// </summary>
        public int Rolls { get; set; }
    }
}
