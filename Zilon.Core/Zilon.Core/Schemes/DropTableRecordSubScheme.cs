namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Запись в схеме таблицы дропа.
    /// </summary>
    /// <remarks>
    /// Содержит информацию о том, какой продемет может выпасть,
    /// количество/качество и с какой вероятностью.
    /// </remarks>
    public sealed class DropTableRecordSubScheme: SubSchemeBase
    {
        public DropTableRecordSubScheme(string schemeSid, int weight)
        {
            SchemeSid = schemeSid;
            Weight = weight;
        }

        /// <summary>
        /// Схема предмета.
        /// </summary>
        public string SchemeSid { get; set; }

        /// <summary>
        /// Вес записи в таблице дропа.
        /// </summary>
        /// <remarks>
        /// Чем выше, тем веротянее будет выбрана данная запись при разрешении дропа.
        /// </remarks>
        public int Weight { get; set; }

        /// <summary>
        /// Минимальная мощь экипировки.
        /// </summary>
        public int MinPower { get; set; }

        /// <summary>
        /// Максимальная мощь экипировки.
        /// </summary>
        public int MaxPower { get; set; }

        /// <summary>
        /// Минимальное количество ресурса.
        /// </summary>
        public int MinCount { get; set; }

        /// <summary>
        /// Максимальное количество ресурса.
        /// </summary>
        public int MaxCount { get; set; }

        /// <summary>
        /// Концепт какого предмета.
        /// </summary>
        /// <remarks>
        /// См. описание сущности концепта.
        /// </remarks>
        public string Concept { get; set; }
    }
}
