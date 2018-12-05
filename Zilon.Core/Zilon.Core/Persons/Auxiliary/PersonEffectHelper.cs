using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

using JetBrains.Annotations;

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
        /// <param name="keyPoints"> Ключевые точки, которые учавствуют в изменении характеристик. </param>
        public static void UpdateSurvivalEffect(
            [NotNull] EffectCollection currentEffects,
            [NotNull] SurvivalStat stat,
            [NotNull] [ItemNotNull] IEnumerable<SurvivalStatKeyPoint> keyPoints,
            [NotNull] ISurvivalRandomSource survivalRandomSource)
        {
            CheckArguments(currentEffects, stat, keyPoints, survivalRandomSource);

            var statType = stat.Type;
            var currentTypeEffect = GetCurrentEffect(currentEffects, statType);

            var keyPoint = keyPoints.Last();

            if (currentTypeEffect != null)
            {
                // Эффект уже существует. Изменим его уровень.
                if (stat.Value <= keyPoint.Value)
                {
                    currentTypeEffect.Level = keyPoint.Level;
                }
                else
                {
                    if (keyPoint.Level == SurvivalStatHazardLevel.Lesser)
                    {
                        currentEffects.Remove(currentTypeEffect);
                    }
                    else
                    {
                        switch (keyPoint.Level)
                        {
                            case SurvivalStatHazardLevel.Strong:
                                currentTypeEffect.Level = SurvivalStatHazardLevel.Lesser;
                                break;

                            case SurvivalStatHazardLevel.Max:
                                currentTypeEffect.Level = SurvivalStatHazardLevel.Strong;
                                break;

                            default:
                                throw new InvalidOperationException("Уровень эффекта, который не обрабатывается.");
                        }
                    }
                }
            }
            else
            {
                // Создаём эффект
                var newCurrentTypeEffect = new SurvivalStatHazardEffect(statType,
                    keyPoint.Level,
                    survivalRandomSource);

                currentEffects.Add(newCurrentTypeEffect);
            }
        }

        private static SurvivalStatHazardEffect GetCurrentEffect(EffectCollection currentEffects, SurvivalStatType statType)
        {
            return currentEffects.Items
                            .OfType<SurvivalStatHazardEffect>()
                            .SingleOrDefault(x => x.Type == statType);
        }

        [ExcludeFromCodeCoverage]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CheckArguments(EffectCollection currentEffects,
            SurvivalStat stat,
            IEnumerable<SurvivalStatKeyPoint> keyPoints,
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
