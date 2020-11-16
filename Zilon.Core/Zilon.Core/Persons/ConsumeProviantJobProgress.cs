using System.Collections.Generic;

using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Прогресс работы по уничтожению цели.
    /// </summary>
    public class ConsumeProviantJobProgress : IJobProgress
    {
        /// <inheritdoc />
        public IJob[] ApplyToJobs(IEnumerable<IJob> currentJobs)
        {
            if (currentJobs is null)
            {
                throw new System.ArgumentNullException(nameof(currentJobs));
            }

            var modifiedJobs = new List<IJob>();

            foreach (var job in currentJobs)
            {
                if (job.Scheme.Type != JobType.ConsumeProviant)
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