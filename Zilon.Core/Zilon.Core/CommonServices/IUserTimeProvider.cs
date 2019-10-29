using System;

namespace Zilon.Core.CommonServices
{
    /// <summary>
    /// Сервиса для работы с локальным пользорвательским временем.
    /// </summary>
    public interface IUserTimeProvider
    {
        /// <summary>
        /// Возвращает текущее локальное пользовательское время.
        /// </summary>
        /// <returns> Структура с текущим временем. </returns>
        DateTime GetCurrentTime();
    }
}
