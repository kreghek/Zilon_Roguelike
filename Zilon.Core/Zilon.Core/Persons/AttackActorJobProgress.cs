using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Newtonsoft.Json;

using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.Persons
{
    public class AttackActorJobProgress : IJobProgress
    {
        private readonly IActor _targetActor;
        private readonly ITacticalAct _tacticalAct;

        public AttackActorJobProgress(IActor targetActor, ITacticalAct tacticalAct)
        {
            _targetActor = targetActor;
            _tacticalAct = tacticalAct;
        }

        public IJob[] ApplyToJobs(IEnumerable<IJob> currentJobs)
        {
            if (currentJobs is null)
            {
                throw new System.ArgumentNullException(nameof(currentJobs));
            }

            var modifiedJobs = new List<IJob>();
            foreach (var job in currentJobs)
            {
                if (job.Scheme.Type != JobType.AttacksActor)
                {
                    continue;
                }

                ProcessJob(job, modifiedJobs);
            }

            return modifiedJobs.ToArray();
        }

        private void ProcessJob(IJob job, List<IJob> modifiedJobs)
        {
            if (job.Scheme.Data == null)
            {
                AddProgress(job, modifiedJobs);
            }
            else
            {
                foreach (var dataItem in job.Scheme.Data)
                {
                    Debug.Assert(!string.IsNullOrWhiteSpace(dataItem), "Данные работы не должны быть пустыми.");
                    if (string.IsNullOrWhiteSpace(dataItem))
                    {
                        continue;
                    }

                    var jobData = JsonConvert.DeserializeObject<AttackActorJobData>(dataItem);

                    Debug.Assert(jobData.MonsterTags != null || jobData.WeaponTags != null,
                        "В данных работ должны быть указаны теги, на основе которых фильтруются работы.");

                    if (jobData.WeaponTags != null)
                    {
                        // Засчитываем прогресс, если у оружия, которым было произведено действие,
                        // есть все указанные теги.

                        ProcessAttackBySpecifiedWeapons(job, modifiedJobs, jobData);
                    }
                    else if (jobData.MonsterTags != null)
                    {
                        ProcessAttackToSpecifiedMonsters(job, modifiedJobs, jobData);
                    }
                    else
                    {
                        Debug.Assert(true, "Все варианты данных должны обрабатываться.");
                    }
                }
            }
        }

        private void ProcessAttackBySpecifiedWeapons(IJob job, List<IJob> modifiedJobs, AttackActorJobData jobData)
        {
            Debug.Assert(jobData.WeaponTags.Any(), "Должно быть указано не менее одного тега.");
            if (jobData.WeaponTags.Any())
            {
                var monsterHasAllTags = true;
                foreach (var tag in jobData.WeaponTags)
                {
                    Debug.Assert(!string.IsNullOrWhiteSpace(tag), "Теги не могут быть пустыми.");
                    if (string.IsNullOrWhiteSpace(tag))
                    {
                        continue;
                    }

                    var monsterPerson = _targetActor.Person as MonsterPerson;

                    if (monsterPerson is null)
                    {
                        monsterHasAllTags = false;
                        break;
                    }

                    if (monsterPerson.Scheme.Tags != null)
                    {
                        if (!monsterPerson.Scheme.Tags.Contains(tag))
                        {
                            monsterHasAllTags = false;
                            break;
                        }
                    }
                }

                if (monsterHasAllTags)
                {
                    AddProgress(job, modifiedJobs);
                }
            }
        }

        private void ProcessAttackToSpecifiedMonsters(IJob job, List<IJob> modifiedJobs, AttackActorJobData jobData)
        {
            Debug.Assert(jobData.WeaponTags.Any(), "Должно быть указано не менее одного тега.");
            if (jobData.WeaponTags.Any())
            {

                var weaponHasAllTags = true;
                foreach (var tag in jobData.WeaponTags)
                {
                    Debug.Assert(!string.IsNullOrWhiteSpace(tag), "Теги не могут быть пустыми.");
                    if (string.IsNullOrWhiteSpace(tag))
                    {
                        continue;
                    }

                    if (_tacticalAct.Equipment == null)
                    {
                        weaponHasAllTags = false;
                        break;
                    }

                    if (_tacticalAct.Equipment.Scheme.Tags != null)
                    {
                        if (!_tacticalAct.Equipment.Scheme.Tags.Contains(tag))
                        {
                            weaponHasAllTags = false;
                            break;
                        }
                    }
                }

                if (weaponHasAllTags)
                {
                    AddProgress(job, modifiedJobs);
                }
            }
        }

        private static void AddProgress(IJob job, List<IJob> modifiedJobs)
        {
            job.Progress++;
            modifiedJobs.Add(job);
        }
    }
}
