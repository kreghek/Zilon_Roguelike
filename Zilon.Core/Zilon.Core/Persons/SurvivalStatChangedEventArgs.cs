using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Аргументы события при изменения характеристики.
    /// </summary>
    public class SurvivalStatChangedEventArgs : EventArgs
    {
        [ExcludeFromCodeCoverage]
        public SurvivalStatChangedEventArgs([NotNull] SurvivalStat stat,
            [NotNull] [ItemNotNull] IEnumerable<SurvivalStatKeyPoint> keyPoints)
        {
            Stat = stat ?? throw new ArgumentNullException(nameof(stat));
            KeyPoints = keyPoints ?? throw new ArgumentNullException(nameof(keyPoints));
        }

        /// <summary>
        /// Характеристика, которая изменялась.
        /// </summary>
        public SurvivalStat Stat { get; }

        /// <summary>
        /// Ключевые точки, которые были пересечены.
        /// </summary>
        public IEnumerable<SurvivalStatKeyPoint> KeyPoints { get; }
    }
}
