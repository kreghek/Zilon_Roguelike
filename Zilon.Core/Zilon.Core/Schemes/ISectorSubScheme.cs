namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Интерфейс схемы для создания сектора.
    /// </summary>
    public interface ISectorSubScheme: ISubScheme
    {
        /// <summary>
        /// Идентфиикаторы обычных монстров, встречаемых в секторе.
        /// </summary>
        string[] RegularMonsterSids { get; }

        /// <summary>
        /// Идентификаторы редких монстров, встречаемых в секторе.
        /// </summary>
        string[] RareMonsterSids { get; }

        /// <summary>
        /// Идентификаторы боссов, встречаемых в секторе.
        /// </summary>
        string[] ChampionMonsterSids { get; }

        /// <summary>
        /// Тип фабрики для карты.
        /// </summary>
        SectorSubSchemeMapFactory MapFactory { get; }

        /// <summary>
        /// Количество регионов в карте.
        /// </summary>
        /// <remarks>
        /// Для подземелий это количество комнат.
        /// </remarks>
        int RegionCount { get; }

        /// <summary>
        /// Максимальный размер комнат.
        /// </summary>
        /// <remarks>
        /// Минимальный размер всегда 2х2.
        /// </remarks>
        int RegionSize { get; }

        /// <summary>
        /// Количество монстров в секторе.
        /// </summary>
        int MonsterCount { get; }

        /// <summary>
        /// Количество сундуков в секторе.
        /// </summary>
        int ChestCount { get; }

        /// <summary>
        /// Таблицы дропа для сундуков.
        /// </summary>
        string[] ChestDropTableSids { get; }
    }
}
