using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Интерфейс для сущностей, выполнение которых зависит от работ (перки, квесты).
    /// </summary>
    public interface IJobExecutable
    {
        IJob[] CurrentJobs { get; }
    }
}
