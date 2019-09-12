using System;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Текущие показатели характеристики модуля выживания.
    /// </summary>
    public sealed class HpSurvivalStat : SurvivalStat
    {
        private float[] _keyPointShares;

        public HpSurvivalStat(int startValue, int min, int max) : base(startValue, min, max)
        {
            _keyPointShares = new[] { 0.75f, 0.5f, 0.25f };
        }

        public override void ChangeStatRange(int min, int max)
        {
            base.ChangeStatRange(min, max);

            var statLength = max - min;

            var factValues = new int[_keyPointShares.Length];
            for (var i = 0; i < _keyPointShares.Length; i++)
            {
                var currentKeyPointFloat = statLength * _keyPointShares[i] + min;
                factValues[i] = (int)Math.Round(currentKeyPointFloat);
            }

            KeyPoints = new[] {
                new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Lesser, factValues[0]),
                new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Strong, factValues[1]),
                new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Max, factValues[2]),
            };
        }
    }
}
