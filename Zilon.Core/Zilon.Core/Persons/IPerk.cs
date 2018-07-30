using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Информация о перке.
    /// </summary>
    public interface IPerk
    {
        /// <summary>
        /// Схема перка.
        /// </summary>
        PerkScheme Scheme { get; }

        /// <summary>
        /// Расчитывает прогресс по заданию.
        /// </summary>
        /// <param name="jobProgress"> Данные по прогрессу по работе. </param>
        void AddProgress(IJobProgress jobProgress);
    }
}
