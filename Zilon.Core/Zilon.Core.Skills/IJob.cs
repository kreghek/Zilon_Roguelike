namespace Zilon.Core.Skills
{
    /// <summary>
    /// Интерфейс произвольной работы и её текущего состояния.
    /// </summary>
    public interface IJob<TScheme> where TScheme: IMinimalJobSubScheme
    {
        /// <summary>
        /// Признак того, что работа завершена.
        /// </summary>
        bool IsComplete { get; set; }

        /// <summary>
        /// Текущий прогресс по работе.
        /// </summary>
        int Progress { get; set; }

        /// <summary>
        /// Схема текущей работы.
        /// </summary>
        TScheme Scheme { get; }
    }
}