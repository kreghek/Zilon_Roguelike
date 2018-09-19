namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема монстра.
    /// </summary>
    public class MonsterScheme: SchemeBase
    {
        /// <summary>
        /// Хитпоинты монстра.
        /// </summary>
        public int Hp { get; }

        /// <summary>
        /// Основное действие монстра.
        /// </summary>
        public TacticalActStatsSubScheme PrimaryAct { get; set; }

        /// <summary>
        /// Список идентификаторов таблиц дропа.
        /// </summary>
        public string[] DropTableSids { get; set; }
    }
}
