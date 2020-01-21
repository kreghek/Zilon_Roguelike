using Zilon.Core.Persons.Survival;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Текущие показатели характеристики модуля выживания.
    /// </summary>
    public sealed class HpSurvivalStat : SurvivalStat
    {
        public HpSurvivalStat(int startValue, int min, int max) : base(startValue, min, max)
        {
            KeySegments = new[] {
                new SurvivalStatKeySegment(0, 0.25f, SurvivalStatHazardLevel.Max),
                new SurvivalStatKeySegment(0.25f, 0.5f, SurvivalStatHazardLevel.Strong),
                new SurvivalStatKeySegment(0.5f, 0.75f, SurvivalStatHazardLevel.Lesser)
            };
        }
    }
}
