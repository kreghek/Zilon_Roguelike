namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Фабрика для обработчиков схем.
    /// </summary>
    /// <remarks>
    /// Нужна для того, чтобы во для тестов можно было генерировать обработчики,
    /// чувствительные к лишним членам в json.
    /// </remarks>
    public interface ISchemeServiceHandlerFactory
    {
        /// <summary>
        /// Создание обработчика схем.
        /// </summary>
        /// <typeparam name="TScheme"> Тип схемы. </typeparam>
        /// <returns> Возвращает обработчик схем. </returns>
        ISchemeServiceHandler<TScheme> Create<TScheme>() where TScheme : class, IScheme;
    }
}