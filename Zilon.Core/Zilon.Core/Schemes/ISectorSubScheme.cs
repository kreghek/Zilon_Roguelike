namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Интерфейс схемы для создания сектора.
    /// </summary>
    public interface ISectorSubScheme : ISubScheme
    {
        /// <summary>
        /// Идентификаторы боссов, встречаемых в секторе.
        /// </summary>
        string[] ChampionMonsterSids { get; }

        /// <summary>
        /// Таблицы дропа для сундуков.
        /// </summary>
        string[] ChestDropTableSids { get; }

        /// <summary>
        /// Описание сектора.
        /// </summary>
        LocalizedStringSubScheme Description { get; }

        /// <summary>
        /// Индикатор того, что сектор является стартовым при входе из локации.
        /// </summary>
        bool IsStart { get; }

        /// <summary>
        /// параметры генерации карты.
        /// </summary>
        ISectorMapFactoryOptionsSubScheme MapGeneratorOptions { get; }

        /// <summary>
        /// Минимальное количество монстров в регионе сектора.
        /// </summary>
        int MinRegionMonsterCount { get; }

        /// <summary>
        /// Наименование сектора.
        /// </summary>
        LocalizedStringSubScheme Name { get; }

        /// <summary>
        /// Идентификаторы редких монстров, встречаемых в секторе.
        /// </summary>
        string[] RareMonsterSids { get; }

        /// <summary>
        /// Коэффициент максимального количества сундуков в регионе сектора (комнате)
        /// в зависимости от размера региона.
        /// </summary>
        int RegionChestCountRatio { get; }

        //TODO Переименовать в MaxRegionMonsterCount.
        /// <summary>
        /// Максимальное количество монстров в регионе сектора.
        /// </summary>
        int RegionMonsterCount { get; }

        /// <summary>
        /// Идентфиикаторы обычных монстров, встречаемых в секторе.
        /// </summary>
        string[] RegularMonsterSids { get; }

        /// <summary>
        /// Символьный идентфиикатор сектора.
        /// </summary>
        /// <remarks>
        /// Нужен для перехода из сектора в сектор.
        /// </remarks>
        string Sid { get; }

        /// <summary>
        /// Максимальное количество сундуков в секторе.
        /// </summary>
        int TotalChestCount { get; }

        /// <summary>
        /// Идентфикаторы связанных секторов в рамках текущей локации.
        /// </summary>
        ISectorTransitionSubScheme[] TransSectorSids { get; }
    }
}