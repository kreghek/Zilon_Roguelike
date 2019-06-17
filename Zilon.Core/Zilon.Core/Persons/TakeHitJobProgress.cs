using System.Collections.Generic;

using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    public sealed class TakeHitJobProgress : IJobProgress
    {
        public IJob[] ApplyToJobs(IEnumerable<IJob> currentJobs)
        {
            var modifiedJobs = new List<IJob>();
            foreach (var job in currentJobs)
            {
                if (job.Scheme.Type != JobType.ReceiveHits)
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
