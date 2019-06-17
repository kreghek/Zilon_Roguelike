using System.Collections.Generic;

using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    public sealed class TakeDamageJobProgress : IJobProgress
    {
        private readonly int _damage;

        public TakeDamageJobProgress(int damage)
        {
            _damage = damage;
        }

        public IJob[] ApplyToJobs(IEnumerable<IJob> currentJobs)
        {
            var modifiedJobs = new List<IJob>();
            foreach (var job in currentJobs)
            {
                if (job.Scheme.Type != JobType.ReceiveDamage)
                {
                    continue;
                }

                job.Progress += _damage;
                modifiedJobs.Add(job);
            }

            return modifiedJobs.ToArray();
        }
    }
}
