using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zilon.Core.Logging;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    public static class PerkResolver
    {
        /// <summary>
        /// Выполняет расчёт прокачки перка на основании его выполненых работ.
        /// </summary>
        /// <param name="activePerk">Активный перк.</param>
        /// <returns>Возвращает true, если перк прокачен хоть на уровень.</returns>
        /// <remarks>
        /// У перка должны быть выставлен текущий прогресс работ в DoneLevelJobs.
        /// </remarks>
        public static bool Resolve(Perk activePerk)
        {
            if (activePerk.DoneLevelJobs == null)
            {
                throw new AppException("Активный перк не содержит никаких работ.");
            }

            if (activePerk.TargetLevelScheme == null)
            {
                throw new AppException("Активным перком выбран перк, которые не может дальше развиваться.");
            }

            var allJobsDone = UpdateJobs(activePerk.TargetLevelScheme.Jobs, activePerk.DoneLevelJobs);

            if (!allJobsDone)
            {
                return false;
            }

            activePerk.DoneLevelJobs = null;

            // Если все работы выполнены, то повышаем подуровень перка.
            // Если взяты все подуровни, то переходим к следующему уровню.
            var currentLevel = activePerk.CurrentLevel;
            var currentSubLevel = activePerk.CurrentSubLevel;

            PerkHelper.GetNextLevel(activePerk.Scheme, ref currentLevel, ref currentSubLevel);

            activePerk.CurrentLevel = currentLevel;
            activePerk.CurrentSubLevel = currentSubLevel;
            activePerk.State = PerkState.None;
            activePerk.IsLevelPaid = false;

            PerkHelper.CalcLevelScheme(activePerk.Scheme.Levels, currentLevel, currentSubLevel,
                out PerkLevelSubScheme archievedLevelScheme, out PerkLevelSubScheme targetLevelScheme);
            activePerk.ArchievedLevelScheme = archievedLevelScheme;
            activePerk.TargetLevelScheme = targetLevelScheme;

            return true;
        }

        /// <summary>
        /// Выполняет обновление текущих работ перка на основе указанных работ.
        /// </summary>
        /// <param name="activePerk">Активный перк.</param>
        /// <param name="actorJobs">Работы, которые были выполнены в рамках этого перка.</param>
        /// <returns>Актеальное состояние выполненых работ перка.</returns>
        public static PerkJob[] UpdateActivePerkProgress(Perk activePerk, PerkJob[] actorJobs)
        {
            var totalPersonJobs = new List<PerkJob>();
            foreach (var schemeJob in activePerk.TargetLevelScheme.Jobs)
            {
                var totalJob = new PerkJob
                {
                    Type = schemeJob.Type,
                    Scope = schemeJob.Scope,
                    Data = schemeJob.Data
                };
                totalPersonJobs.Add(totalJob);

                var foundActorJobs = actorJobs.Where(x => x.Scope == schemeJob.Scope && x.Type == schemeJob.Type).ToArray();
                if (foundActorJobs.Count() > 1)
                {
                    Logger.TraceError(LogCodes.ErrorCommon, "Для задачи перка актёра найдено больше одной задачи из схемы.");
                }
                var actorJob = foundActorJobs.FirstOrDefault();

                var foundPersonJobs = activePerk.DoneLevelJobs.Where(x => x.Scope == schemeJob.Scope && x.Type == schemeJob.Type).ToArray();
                if (foundPersonJobs.Count() > 1)
                {
                    Logger.TraceError(LogCodes.ErrorCommon, "Для задачи перка персонажа найдено больше одной задачи из схемы.");
                }
                var personJob = foundPersonJobs.FirstOrDefault();

                if (personJob != null)
                {
                    if (actorJob != null)
                    {
                        if (schemeJob.Scope == JobScope.Global)
                        {
                            totalJob.Progress = personJob.Progress + actorJob.Progress;
                        }
                        else if (schemeJob.Scope == JobScope.Scenario && personJob.Progress < actorJob.Progress)
                        {
                            totalJob.Progress = actorJob.Progress;
                        }
                    }
                    else
                    {
                        totalJob.Progress = personJob.Progress;
                    }
                }
                else
                {
                    if (actorJob != null)
                    {
                        totalJob.Progress = actorJob.Progress;
                    }
                }
            }

            return totalPersonJobs.ToArray();
        }

        /// <summary>
        /// Выполняет расчёт и обновление только работ перка.
        /// </summary>
        /// <param name="schemeJobs">Работы схемы.</param>
        /// <param name="factJobs">Фактические работы. Будут модифицированы.</param>
        /// <returns>true - если все работы перка прокачены. Иначе - false.</returns>
        //TODO Сделать отдельно метод, который выставляет признак Complete для задачи и метод проверки перка на выполнение всех условий.
        public static bool UpdateJobs(IEnumerable<PerkJob> schemeJobs, IEnumerable<PerkJob> factJobs)
        {
            if (!schemeJobs.Any())
            {
                throw new AppException("Обновление перка, у которого не заданы работы.");
            }

            var schemeJobArray = schemeJobs.ToArray();
            var factJobArray = factJobs.ToArray();

            var allJobsDone = true;
            for (var i = 0; i < schemeJobArray.Length; i++)
            {
                var schemeJob = schemeJobArray[i];
                var factJob = factJobArray.SingleOrDefault(x => x.Type == schemeJob.Type && x.Scope == schemeJob.Scope);

                if (factJob == null)
                    continue;

                if (factJob.Progress < schemeJob.Progress)
                {
                    allJobsDone = false;
                }
                else
                {
                    factJobArray[i].IsComplete = true;
                }
            }

            return allJobsDone;
        }
    }
}
