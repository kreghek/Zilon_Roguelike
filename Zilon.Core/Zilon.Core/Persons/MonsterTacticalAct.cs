using System;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Тактическое действие для монстров.
    /// </summary>
    public class MonsterTacticalAct : ITacticalAct
    {
        public TacticalActStatsSubScheme Stats { get; }

        public float MinEfficient { get; }

        public float MaxEfficient { get; }

        public MonsterTacticalAct(TacticalActStatsSubScheme stats, float multiplier)
        {
            Stats = stats;
            MinEfficient = CalcEfficient(stats.Efficient.Min, multiplier);
            MaxEfficient = CalcEfficient(stats.Efficient.Max, multiplier);
        }

        private float CalcEfficient(float baseEfficient,
            float multiplier)
        {
            return baseEfficient * multiplier;
        }
    }
}
