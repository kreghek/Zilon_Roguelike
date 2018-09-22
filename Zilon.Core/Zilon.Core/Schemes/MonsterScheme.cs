using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема монстра.
    /// </summary>
    public class MonsterScheme : SchemeBase
    {
        /// <summary>
        /// Хитпоинты монстра.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public int Hp { get; set; }

        /// <summary>
        /// Основное действие монстра.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public TacticalActStatsSubScheme PrimaryAct { get; set; }

        /// <summary>
        /// Список идентификаторов таблиц дропа.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public string[] DropTableSids { get; set; }
    }
}
