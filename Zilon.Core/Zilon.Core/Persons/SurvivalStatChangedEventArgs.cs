using System;
using System.Collections.Generic;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Аргументы события при изменения характеристики.
    /// </summary>
    public class SurvivalStatChangedEventArgs: EventArgs
    {
        public SurvivalStatChangedEventArgs(SurvivalStat stat,
            IEnumerable<SurvivalStatKeyPoint> keyPoints) {
            Stat = stat;
            KeyPoints = keyPoints;
        }

        public SurvivalStat Stat { get; }
        public IEnumerable<SurvivalStatKeyPoint> KeyPoints { get; }
    }
}
