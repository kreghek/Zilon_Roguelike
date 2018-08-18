using System;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Аргументы события при изменения характеристики.
    /// </summary>
    public class SurvivalStatChangedEventArgs: EventArgs
    {
        public SurvivalStatChangedEventArgs(SurvivalStat stat) {
            Stat = stat;
        }

        public SurvivalStat Stat { get; }
    }
}
