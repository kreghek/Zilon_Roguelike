using Zilon.Core.Schemes;

namespace Zilon.Core
{
    /// <summary>
    /// Интерфейс произвольной работы и её текущего состояния.
    /// </summary>
    public interface IJob
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
        IJobSubScheme Scheme { get; }
    }
}