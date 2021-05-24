using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Zilon.Core.PersonModules;
using Zilon.Core.Persons.Survival;
using Zilon.Core.Scoring;

namespace Zilon.Core.Persons.Auxiliary
{
    /// <summary>
    /// Вспомогательный класс для работы с эффектами персонажа.
    /// </summary>
    public static class PersonConditionHelper
    {
        /// <summary>
        /// Обновление эффекта модуля выживания.
        /// </summary>
        /// <param name="currentCondition"> Текущий список условий. </param>
        /// <param name="stat"> Характеристика, на которую влияет эффект. </param>
        /// <param name="keySegments">
        /// Ключевые сегменты, которые были пересечены при изменении характеристики.
        /// <param name="survivalRandomSource"> Источник рандома выживания. </param>
        public static void UpdateSurvivalСondition(
            [NotNull] IConditionsModule currentCondition,
            [NotNull] SurvivalStat stat,
            [NotNull] SurvivalStatKeySegment[] keySegments,
            [NotNull] ISurvivalRandomSource survivalRandomSource,
            [MaybeNull] IPlayerEventLogService? playerEventLogService)
        {
            ThrowExceptionIfArgumentsInvalid(currentCondition, stat, keySegments, survivalRandomSource);

            // Эффект выставляем на основе текущего ключевого сегмента, в которое попадает значение характеристики выживания.
            // Если текущее значение не попадает ни в один сегмент, то эффект сбрасывается.

            var currentSegments = keySegments.CalcIntersectedSegments(stat.ValueShare);

            // Если попадаем на стык с двумя сегментами, просто берём первый.
            // Иногда это будет давать более сильный штрафной эффект,
            // но пока не понятно, как по другому сделать отрезки.
            var currentSegment = currentSegments.FirstOrDefault();

            var statType = stat.Type;
            var currentTypeСondition = GetCurrentСondition(currentCondition, statType);
            if (currentTypeСondition != null)
            {
                // Эффект уже существует. Изменим его уровень.
                // Или удалим, если текущее значение не попадает ни в один из сегментов.
                if (currentSegment == null)
                {
                    currentCondition.Remove(currentTypeСondition);
                }
                else
                {
                    currentTypeСondition.Level = currentSegment.Level;
                }
            }
            else
            {
                if (currentSegment != null)
                {
                    // Создаём эффект
                    var newEffect = new SurvivalStatHazardCondition(
                        statType,
                        currentSegment.Level,
                        survivalRandomSource)
                    {
                        PlayerEventLogService = playerEventLogService
                    };

                    currentCondition.Add(newEffect);
                }
            }
        }

        private static SurvivalStatHazardCondition GetCurrentСondition(IConditionsModule сonditionModule,
            SurvivalStatType statType)
        {
            return сonditionModule.Items
                .OfType<SurvivalStatHazardCondition>()
                .SingleOrDefault(x => x.Type == statType);
        }

        [ExcludeFromCodeCoverage]
        private static void ThrowExceptionIfArgumentsInvalid(IConditionsModule currentСondition,
            SurvivalStat stat,
            IEnumerable<SurvivalStatKeySegment> keyPoints,
            ISurvivalRandomSource survivalRandomSource)
        {
            if (currentСondition == null)
            {
                throw new ArgumentNullException(nameof(currentСondition));
            }

            if (stat == null)
            {
                throw new ArgumentNullException(nameof(stat));
            }

            if (keyPoints == null)
            {
                throw new ArgumentNullException(nameof(keyPoints));
            }

            if (survivalRandomSource == null)
            {
                throw new ArgumentNullException(nameof(survivalRandomSource));
            }
        }
    }
}