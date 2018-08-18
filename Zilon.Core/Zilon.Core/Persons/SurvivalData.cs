using System;
using System.Linq;

using Zilon.Core.Common;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Базовая реализация данных о выживании.
    /// </summary>
    public class SurvivalData : ISurvivalData
    {
        public SurvivalData()
        {
            Stats = new[] {
                new SurvivalStat{
                    Type = SurvivalStatTypes.Satiety,
                    Range = new Range<int>(-100, 100),
                    Rate = 1,
                    Value = 50
                },
                new SurvivalStat{
                    Type = SurvivalStatTypes.Water,
                    Range = new Range<int>(-100, 100),
                    Rate = 1,
                    Value = 50
                }
            };
        }

        public SurvivalStat[] Stats { get; }

        public event EventHandler<SurvivalStatChangedEventArgs> StatCrossKeyValue;

        public void RestoreStat(SurvivalStatTypes type, int value)
        {
            var stat = Stats.SingleOrDefault(x => x.Type == type);
            if (stat != null)
            {
                stat.Value += value;

                if (stat.Value >= stat.Range.Max)
                {
                    stat.Value = stat.Range.Max;
                }
            }
        }

        public void Update()
        {
            foreach (var stat in Stats)
            {
                stat.Value -= stat.Rate;

                if (stat.Value <= stat.Range.Min)
                {
                    stat.Value = stat.Range.Min;
                }
            }
        }
    }
}
