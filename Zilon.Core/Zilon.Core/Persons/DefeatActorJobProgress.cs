using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.Persons
{
    /// <summary>
    ///     Прогресс работы по уничтожению цели.
    /// </summary>
    public class DefeatActorJobProgress : IJobProgress
    {
        public DefeatActorJobProgress(IActor target)
        {
            Target = target;
        }

        public IActor Target { get; }

        public IJob[] ApplyToJobs(IEnumerable<IJob> currentJobs)
        {
            if (currentJobs is null)
            {
                throw new System.ArgumentNullException(nameof(currentJobs));
            }

            var modifiedJobs = new List<IJob>();
            foreach (var job in currentJobs)
            {
                if (job.Scheme.Type != JobType.Defeats)
                {
                    continue;
                }

                job.Progress++;
                modifiedJobs.Add(job);
            }

            return modifiedJobs.ToArray();
        }
    }
}