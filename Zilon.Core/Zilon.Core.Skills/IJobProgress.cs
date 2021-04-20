using System.Collections.Generic;

namespace Zilon.Core.Skills
{
    /// <summary>
    /// Интерфейс для прогресса по работе.
    /// </summary>
    /// <remarks>
    /// Используется, чтобы зафиксировать выполнение работы или её части.
    /// </remarks>
    public interface IJobProgress<TJobScheme> where TJobScheme : IMinimalJobSubScheme
    {
        /// <summary>
        /// Применяет прогресс к указанным работам.
        /// </summary>
        /// <param name="currentJobs"> Текущий набор работ, к которым необходимо применить прогресс. </param>
        /// <returns> Возвращает набор работ, которые были изменены. </returns>
        IJob<TJobScheme>[] ApplyToJobs(IEnumerable<IJob<TJobScheme>> currentJobs);
    }
}