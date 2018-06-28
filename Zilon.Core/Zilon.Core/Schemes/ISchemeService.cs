namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Сервис для работы со схемами.
    /// </summary>
    public interface ISchemeService
    {
        /// <summary>
        /// Извлечь схему по идентификатору.
        /// </summary>
        /// <typeparam name="TScheme"> Тип схемы. </typeparam>
        /// <param name="sid"> Идентификатор схемы. </param>
        /// <returns> Возвращает экземпляр схемы. </returns>
        TScheme GetScheme<TScheme>(string sid) where TScheme: class, IScheme;

        /// <summary>
        /// Извлечь все схемы укаканного типа.
        /// </summary>
        /// <typeparam name="TScheme"> Тип схемы. </typeparam>
        /// <returns> Возвращает набор схем указанного типа. </returns>
        TScheme[] GetSchemes<TScheme>() where TScheme : class, IScheme;
    }
}
