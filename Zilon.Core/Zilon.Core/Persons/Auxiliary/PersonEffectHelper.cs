using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.PersonModules;
using Zilon.Core.Persons.Survival;
using Zilon.Core.Scoring;

namespace Zilon.Core.Persons.Auxiliary
{
    /// <summary>
    /// Вспомогательный класс для работы с эффектами персонажа.
    /// </summary>
    public static class PersonEffectHelper
    {
        /// <summary>
        /// Обновление эффекта модуля выживания.
        /// </summary>
        /// <param name="currentEffects"> Текущий список эффектов. </param>
        /// <param name="stat"> Характеристика, на которую влияет эффект. </param>
        /// <param name="keySegments"> Ключевые сегменты, которые были пересечены при изменении характеристики.
        /// <param name="survivalRandomSource"> Источник рандома выживания. </param>
        public static void UpdateSurvivalEffect(
            [NotNull] IEffectsModule currentEffects,
            [NotNull] SurvivalStat stat,
            [NotNull][ItemNotNull] SurvivalStatKeySegment[] keySegments,
            [NotNull] ISurvivalRandomSource survivalRandomSource,
            [NotNull] IPlayerEventLogService playerEventLogService)
        {
            ThrowExceptionIfArgumentsInvalid(currentEffects, stat, keySegments, survivalRandomSource);

            // Эффект выставляем на основе текущего ключевого сегмента, в которое попадает значение характеристики выживания.
            // Если текущее значение не попадает ни в один сегмент, то эффект сбрасывается.

            var currentSegments = keySegments.CalcIntersectedSegments(stat.ValueShare);

            // Если попадаем на стык с двумя сегментами, просто берём первый.
            // Иногда это будет давать более сильный штрафной эффект,
            // но пока не понятно, как по другому сделать отрезки.
            var currentSegment = currentSegments.FirstOrDefault();

            var statType = stat.Type;
            var currentTypeEffect = GetCurrentEffect(currentEffects, statType);
            if (currentTypeEffect != null)
            {
                // Эффект уже существует. Изменим его уровень.
                // Или удалим, если текущее значение не попадает ни в один из сегментов.
                if (currentSegment == null)
                {
                    currentEffects.Remove(currentTypeEffect);
                }
                else
                {
                    currentTypeEffect.Level = currentSegment.Level;
                }
            }
            else
            {
                if (currentSegment != null)
                {
                    // Создаём эффект
                    var newEffect = new SurvivalStatHazardEffect(
                        statType,
                        currentSegment.Level,
                        survivalRandomSource)
                    {
                        PlayerEventLogService = playerEventLogService
                    };

                    currentEffects.Add(newEffect);
                }
            }
        }

        private static SurvivalStatHazardEffect GetCurrentEffect(IEffectsModule currentEffects, SurvivalStatType statType)
        {
            return currentEffects.Items
                            .OfType<SurvivalStatHazardEffect>()
                            .SingleOrDefault(x => x.Type == statType);
        }

        [ExcludeFromCodeCoverage]
        private static void ThrowExceptionIfArgumentsInvalid(IEffectsModule currentEffects,
            SurvivalStat stat,
            IEnumerable<SurvivalStatKeySegment> keyPoints,
            ISurvivalRandomSource survivalRandomSource)
        {
            if (currentEffects == null)
            {
                throw new ArgumentNullException(nameof(currentEffects));
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