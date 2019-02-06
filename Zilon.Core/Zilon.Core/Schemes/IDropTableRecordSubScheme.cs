namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Запись в таблице дропа.
    /// </summary>
    public interface IDropTableRecordSubScheme : ISubScheme
    {
        /// <summary>
        /// Максимальное количество ресурсов.
        /// </summary>
        /// <remarks>
        /// Используется только ресурсами.
        /// </remarks>
        int MaxCount { get; }

        /// <summary>
        /// Минимальное количество ресурсов.
        /// </summary>
        /// <remarks>
        /// Используется только ресурсами.
        /// </remarks>
        int MinCount { get; }

        /// <summary>
        /// Идентификатор схемы предмета.
        /// </summary>
        /// <remarks>
        /// Если указано null, то никакой предмет не дропается. null - это ничего не дропнулось.
        /// </remarks>
        string SchemeSid { get; }

        /// <summary>
        /// Вес записи в таблице дропа.
        /// </summary>
        /// <remarks>
        /// Чем выше, тем веротянее будет выбрана данная запись при разрешении дропа.
        /// </remarks>
        int Weight { get; }

        /// <summary>
        /// Дополнительный дроп к текущему.
        /// </summary>
        IDropTableScheme[] Extra { get; }
    }
}