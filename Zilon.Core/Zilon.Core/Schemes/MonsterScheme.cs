using System;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема монстра.
    /// </summary>
    public class MonsterScheme : SchemeBase
    {
        public MonsterScheme(int hp, TacticalActStatsSubScheme primaryAct, MonsterDefenceScheme defence, string[] dropTableSids)
        {
            Hp = hp;
            PrimaryAct = primaryAct ?? throw new ArgumentNullException(nameof(primaryAct));
            Defence = defence ?? throw new ArgumentNullException(nameof(defence));
            DropTableSids = dropTableSids ?? throw new ArgumentNullException(nameof(dropTableSids));
        }

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
