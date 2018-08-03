using System.Collections.Generic;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Интерфейс для прогресса по работе.
    /// </summary>
    /// <remarks>
    /// Используется, чтобы зафиксировать выполнение работы или её части.
    /// </remarks>
    public interface IJobProgress
    {
        /// <summary>
        /// Применяет прогресс к указанным работам.
        /// </summary>
        /// <returns> Возвращает набор работ, которые были изменены. </returns>
        PerkJob[] ApplyToJobs(IEnumerable<PerkJob> currentJobs);
    }
}
