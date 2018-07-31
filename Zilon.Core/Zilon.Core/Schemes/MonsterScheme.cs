namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема монстра.
    /// </summary>
    public class MonsterScheme: SchemeBase
    {
        /// <summary>
        /// Основное действие монстра.
        /// </summary>
        public TacticalActStatsSubScheme PrimaryAct { get; set; }
    }
}
