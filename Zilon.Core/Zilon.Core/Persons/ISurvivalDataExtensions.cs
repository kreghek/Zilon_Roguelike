using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Common;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Вспомогательный класс для работы с <see cref="ISurvivalData"/>.
    /// </summary>
    public static class ISurvivalDataExtensions
    {
        /// <summary>Рассчитывает все ключевые точки в указанном диапазоне.</summary>
        /// <param name="keyPoints">Набор ключевых точек.</param>
        /// <param name="leftValue">The left value.</param>
        /// <param name="rightValue">The right value.</param>
        /// <returns> Возвращает набор точек, значения которых попадает в указанный диапазон.
        /// Точки возвращаются в том порядке, в котором пересекаются, если двигаться от левого значения к правому. </returns>
        public static IEnumerable<SurvivalStatKeyPoint> CalcKeyPointsInRange(
            this SurvivalStatKeyPoint[] keyPoints,
            int leftValue,
            int rightValue)
        {
            var diff = RangeHelper.CreateNormalized(leftValue, rightValue);

            var orientedKeyPoints = keyPoints;
            if (!diff.IsAcs)
            {
                orientedKeyPoints = keyPoints.Reverse().ToArray();
            }

            var crossedKeyPoints = new List<SurvivalStatKeyPoint>();
            foreach (var keyPoint in orientedKeyPoints)
            {
                if (diff.Contains(keyPoint.Value))
                {
                    crossedKeyPoints.Add(keyPoint);
                }
            }

            return crossedKeyPoints;
        }
    }
}
