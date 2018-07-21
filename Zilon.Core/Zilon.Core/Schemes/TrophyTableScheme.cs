namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема тарблицы дропа.
    /// </summary>
    public sealed class TrophyTableScheme: SchemeBase
    {
        public TrophyTableRecordSubScheme[] Records { get; set; }
        public int Rolls { get; set; }
    }
}
