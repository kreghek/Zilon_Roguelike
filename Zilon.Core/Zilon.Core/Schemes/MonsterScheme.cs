using JetBrains.Annotations;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема монстра.
    /// </summary>
    public class MonsterScheme : SchemeBase, IMonsterScheme
    {
        public MonsterScheme(int hp,
            [CanBeNull] TacticalActStatsSubScheme primaryAct,
            [CanBeNull] MonsterDefenceSubScheme defence,
            string[] dropTableSids)
        {
            Hp = hp;
            PrimaryAct = primaryAct;
            Defense = defence;
            DropTableSids = dropTableSids;
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
        public IMonsterDefenseSubScheme Defense { get; }

        /// <summary>
        /// Список идентификаторов таблиц дропа.
        /// </summary>
        public string[] DropTableSids { get; set; }
    }
}
