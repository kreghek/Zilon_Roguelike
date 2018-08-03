using Zilon.Core.Schemes;

namespace Zilon.Core
{
    /// <summary>
    /// Интерфейс произвольной работы и её текущего состояния.
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// Схема текущей работы.
        /// </summary>
        JobSubScheme Scheme { get; }

        /// <summary>
        /// Текущий прогресс по работе.
        /// </summary>
        int Progress { get; set; }

        /// <summary>
        /// Признак того, что работа завершена.
        /// </summary>
        bool IsComplete { get; set; }
    }
}
