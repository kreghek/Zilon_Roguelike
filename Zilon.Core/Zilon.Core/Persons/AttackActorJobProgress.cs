using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var modifiedJobs = new List<IJob>();
            foreach (var job in currentJobs)
            {
                if (job.Scheme.Type != JobType.AttacksActor)
                {
                    continue;
                }

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
                        else if (jobData.MonsterTags != null)
                        {
                            // TODO Сделать аналогично тегам оружия.
                            // Возможно, можно будет ввести общий интерфейс IHasTags/ITaggable (поискать варианты получше) для схем.
                            // И выделить обработку наличия тегов у схему в единый метод, вызываемый для разных схем.
                        }
                        else
                        {
                            Debug.Assert(true, "Все варианты данных должны обрабатываться.");
                        }

                    }
                }
                
            }

            return modifiedJobs.ToArray();
        }

        private static void AddProgress(IJob job, List<IJob> modifiedJobs)
        {
            job.Progress++;
            modifiedJobs.Add(job);
        }
    }
}
