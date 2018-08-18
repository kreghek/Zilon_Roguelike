using System;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Аргументы события при изменения характеристики.
    /// </summary>
    public class SurvivalStatChangedEventArgs: EventArgs
    {
        public SurvivalStatChangedEventArgs(SurvivalStat stat, SurvivalStatKeyPoint keyPoint) {
            Stat = stat;
            KeyPoint = keyPoint;
        }

        public SurvivalStat Stat { get; }
        public SurvivalStatKeyPoint KeyPoint { get; }
    }
}
