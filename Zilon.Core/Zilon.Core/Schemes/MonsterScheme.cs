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
        public int Hp { get; set; }

        /// <summary>
        /// Основное действие монстра.
        /// </summary>
        public ITacticalActStatsSubScheme PrimaryAct { get; set; }

        /// <summary>
        /// Способности к обороне монстра против атакующих действий противника.
        /// </summary>
        public IMonsterDefenceScheme Defence { get; }

        /// <summary>
        /// Список идентификаторов таблиц дропа.
        /// </summary>
        public string[] DropTableSids { get; set; }
    }
}
