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
        /// <param name="currentJobs"> Текущий набор работ, к которым необходимо применить прогресс. </param>
        /// <returns> Возвращает набор работ, которые были изменены. </returns>
        IJob[] ApplyToJobs(IEnumerable<IJob> currentJobs);
    }
}
