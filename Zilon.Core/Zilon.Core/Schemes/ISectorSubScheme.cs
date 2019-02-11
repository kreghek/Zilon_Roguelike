namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Интерфейс схемы для создания сектора.
    /// </summary>
    public interface ISectorSubScheme : ISubScheme
    {
        /// <summary>
        /// Символьный идентфиикатор сектора.
        /// </summary>
        /// <remarks>
        /// Нужен для перехода из сектора в сектор.
        /// </remarks>
        string Sid { get; }

        /// <summary>
        /// Наименование сектора.
        /// </summary>
        LocalizedStringSubScheme Name { get; }

        /// <summary>
        /// Описание сектора.
        /// </summary>
        LocalizedStringSubScheme Description { get; }

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
        /// Максимальное количество монстров в регионе сектора.
        /// </summary>
        int RegionMonsterCount { get; }

        /// <summary>
        /// Количество сундуков в секторе.
        /// </summary>
        int ChestCount { get; }

        /// <summary>
        /// Таблицы дропа для сундуков.
        /// </summary>
        string[] ChestDropTableSids { get; }

        /// <summary>
        /// Идентфикаторы связанных секторов в рамках текущей локации.
        /// </summary>
        string[] TransSectorSids { get; }

        /// <summary>
        /// Индикатор того, что сектор является стартовым при входе из локации.
        /// </summary>
        bool IsStart { get; }
    }
}
