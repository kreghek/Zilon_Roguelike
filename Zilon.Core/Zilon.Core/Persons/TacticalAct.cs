using System.Linq;

using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Тактическое действие актёров под управлением игрока.
    /// </summary>
    public class TacticalAct : ITacticalAct
    {
        public TacticalAct(float equipmentPower, TacticalActScheme scheme, ICombatStats stats)
        {
            Stats = scheme.Stats;
            MinEfficient = CalcEfficient(Stats.Efficient.Min, scheme, equipmentPower, stats);
            MaxEfficient = CalcEfficient(Stats.Efficient.Max, scheme, equipmentPower, stats);
        }

        public TacticalActStatsSubScheme Stats { get; }

        public float MinEfficient { get; }

        public float MaxEfficient { get; }

        private float CalcEfficient(float baseEfficient, 
            TacticalActScheme scheme, 
            float equipmentPower, 
            ICombatStats stats)
        {
            var sum = 0f;

            foreach(var depItem in scheme.Dependency)
            {
                var factStat = stats.Stats.SingleOrDefault(x => x.Stat == depItem.Stat);
                if (factStat == null)
                {
                    continue;
                }

                var factStatValue = factStat.Value / 10f;

                var depShare = factStatValue * depItem.Value;
                var depEfficient = baseEfficient * equipmentPower * factStatValue;
                sum += depEfficient;
            }

            return sum;
        }
    }
}
