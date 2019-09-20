using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Common;

namespace Zilon.Core.Persons.Survival
{
    /// <summary>
    /// Расширения для работы с сегментами характеристик выживания.
    /// </summary>
    public static class SurvivalStatKeySegmentExtensions
    {
        /// <summary>
        /// Рассчёт ключевых сегментов, которые были пересечены при изменении характеристики.
        /// </summary>
        /// <param name="survivalStatKeySegments"> Ключевые сегменты характеристики. </param>
        /// <param name="startStatValueShare"> Начальное значение характеристики выживания. </param>
        /// <param name="endStatValueShare"> Изменённое значение характеристики выживания. </param>
        /// <returns> Возвращает набор пересеченных сегментов в порядке их пересечения. </returns>
        public static SurvivalStatKeySegment[] CalcIntersectedSegments(
            this IEnumerable<SurvivalStatKeySegment> survivalStatKeySegments,
            float startStatValueShare,
            float endStatValueShare)
        {
            var normStart = startStatValueShare;
            var normEnd = endStatValueShare;
            var upRise = true;

            // Нам нужно гарантировать, что все начальные точки меньше или равны конечным.
            if (startStatValueShare > endStatValueShare)
            {
                normEnd = startStatValueShare;
                normStart = endStatValueShare;
                upRise = false;
            }

            var intersectedKeySegments = survivalStatKeySegments
                .Where(x => SegmentHelper.IsIntersects(normStart, normEnd, x.Start, x.End - float.MinValue))
                .ToArray();

            var totalIntersectedKeySegments = intersectedKeySegments;
            if (!upRise)
            {
                totalIntersectedKeySegments = intersectedKeySegments.Reverse().ToArray();
            }

            return totalIntersectedKeySegments;
        }
    }
}
