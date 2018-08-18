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
        /// <param name="keyPoint"> Ключевая точка, которую учавствует в изменении характеристики. </param>
        public static void UpdateSurvivalEffect(IList<IPersonEffect> currentEffects,
            SurvivalStat stat,
            SurvivalStatKeyPoint keyPoint)
        {
            var statType = stat.Type;

            var currentTypeEffect = currentEffects.OfType<SurvivalStatHazardEffect>()
                .SingleOrDefault(x => x.Type == statType);

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
                        }
                    }
                }
            }
            else
            {
                currentTypeEffect = new SurvivalStatHazardEffect(statType, keyPoint.Level);
                currentEffects.Add(currentTypeEffect);
            }
        }
    }
}
