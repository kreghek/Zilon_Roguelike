using System;
using System.Collections.Generic;
using System.Linq;

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
        public static void UpdateSurvivalEffect(EffectCollection currentEffects,
            SurvivalStat stat,
            IEnumerable<SurvivalStatKeyPoint> keyPoints)
        {
            var statType = stat.Type;

            var currentTypeEffect = currentEffects.Items
                .OfType<SurvivalStatHazardEffect>()
                .SingleOrDefault(x => x.Type == statType);

            var keyPoint = keyPoints.First();

            // Эффект уже существует.
            // Изменим его тип.
            if (currentTypeEffect != null)
            {
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
                var newCurrentTypeEffect = new SurvivalStatHazardEffect(statType, keyPoint.Level);
                currentEffects.Add(newCurrentTypeEffect);
            }
        }
    }
}
