namespace Zilon.Core.Skills
{
    /// <summary>
    /// Интерфейс для сущностей, выполнение которых зависит от работ (перки, квесты).
    /// </summary>
    public interface IJobExecutable<TJobScheme> where TJobScheme: IMinimalJobSubScheme
    {
        IJob<TJobScheme>[]? CurrentJobs { get; }
    }
}