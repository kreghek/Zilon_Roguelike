using System.Linq;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    public class TacticalAct : ITacticalAct
    {
        public TacticalAct(float equipmentPower, TacticalActScheme scheme, ICombatStats stats)
        {
            Scheme = scheme;
            MinEfficient = CalcEfficient(scheme.Efficient.Min, scheme, equipmentPower, stats);
            MaxEfficient = CalcEfficient(scheme.Efficient.Max, scheme, equipmentPower, stats);
        }

        public TacticalActScheme Scheme { get; }

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
