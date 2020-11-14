namespace Zilon.Core.CommonServices
{
    /// <summary>
    /// Базовая реализация сервиса для работы с локальным пользорвательским временем.
    /// </summary>
    public class UserTimeProvider : IUserTimeProvider
    {
        /// <summary>
        /// Возвращает текущее локальное пользовательское время.
        /// </summary>
        /// <returns> Структура с текущим временем. </returns>
        public DateTime GetCurrentTime()
        {
            return DateTime.Now;
        }
    }
}