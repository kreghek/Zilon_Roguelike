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
        public static IEnumerable<SurvivalStatKeySegment> CalcIntersectedSegments(
            this IEnumerable<SurvivalStatKeySegment> survivalStatKeySegments,
            float startStatValueShare,
            float endStatValueShare)
        {
            var normStart = startStatValueShare;
            var normEnd = endStatValueShare;

            // Нам нужно гарантировать, что все начальные точки меньше или равны конечным.
            if (startStatValueShare > endStatValueShare)
            {
                normEnd = startStatValueShare;
                normStart = endStatValueShare;
            }

            return survivalStatKeySegments
                .Where(x => SegmentHelper.IsIntersects(normStart, normEnd, x.Start, x.End));
        }

        public static IEnumerable<SurvivalStatKeySegment> CalcIntersectedSegments(
            this IEnumerable<SurvivalStatKeySegment> survivalStatKeySegments,
            float statValueShare)
        {
            return CalcIntersectedSegments(survivalStatKeySegments, statValueShare, statValueShare);
        }
    }
}