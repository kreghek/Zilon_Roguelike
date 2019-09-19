using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

using JetBrains.Annotations;
using Zilon.Core.Persons.Survival;

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
        /// <param name="keyPoints"> Ключевые точки, которые учавствуют в изменении характеристик.
        /// Передаются ключевые точки в порядке их прохождения.</param>
        /// <param name="survivalRandomSource"> Источник рандома выживания. </param>
        public static void UpdateSurvivalEffect(
            [NotNull] EffectCollection currentEffects,
            [NotNull] SurvivalStat stat,
            [NotNull] [ItemNotNull] SurvivalStatKeyPoint[] keyPoints,
            [NotNull] ISurvivalRandomSource survivalRandomSource)
        {
            CheckArguments(currentEffects, stat, keyPoints, survivalRandomSource);

            var statType = stat.Type;
            var currentTypeEffect = GetCurrentEffect(currentEffects, statType);

            // Эффект выставляем на основе последней проёденной ключевой точки.
            // Потому что сюда передаются ключевые точки в порядке их прохождения.
            var keyPoint = keyPoints.Last();

            if (currentTypeEffect != null)
            {
                // Эффект уже существует. Изменим его уровень.

                // ! Костыль. Нужно переделать на отрезки вместо ключевых точек.
                // Так мы определяем поведение в зависимости от характеристики.
                // По сути, для сытости и упитости будем использовать старый алгоритм (чем ниже значение, тем выше уровень угрозы).
                // А для интоксикации, наоборот, чем выше значение.
                // Это возможно, потому что сейчас все ключевые точки расположены либо слева лиюо справа от нуля.
                // Дальше нужно будет переделать на отрезки, когда будут как положительные, так и отрицательные ключевые точки.
                // Это произойдёт, когда будет введено, например, обжорство. Ключевая точка, когда персонаж употребил слишком много еды.
                var upRise = keyPoint.Value <= 0 ? stat.Value <= keyPoint.Value : stat.Value >= keyPoint.Value;

                if (upRise)
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

                            case SurvivalStatHazardLevel.Undefined:
                            case SurvivalStatHazardLevel.Lesser:
                            default:
                                // Для Lesser уже выполняется обработка выше.
                                // Для остальных уровней - в отдельных блоках case.
                                throw new NotSupportedException("Уровень эффекта, который не обрабатывается.");
                        }
                    }
                }
            }
            else
            {
                // Создаём эффект
                var newEffect = new SurvivalStatHazardEffect(
                    statType,
                    keyPoint.Level,
                    survivalRandomSource);

                currentEffects.Add(newEffect);
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
