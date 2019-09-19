using System;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

using Zilon.Core.Persons.Survival;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Аргументы события при изменения характеристики.
    /// </summary>
    public class SurvivalStatChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="stat"></param>
        /// <param name="keySegments"></param>
        [ExcludeFromCodeCoverage]
        public SurvivalStatChangedEventArgs([NotNull] SurvivalStat stat,
            [NotNull] [ItemNotNull] SurvivalStatKeySegment[] keySegments)
        {
            Stat = stat ?? throw new ArgumentNullException(nameof(stat));
            KeySegments = keySegments ?? throw new ArgumentNullException(nameof(keySegments));
        }

        /// <summary>
        /// Характеристика, которая изменялась.
        /// </summary>
        public SurvivalStat Stat { get; }

        /// <summary>
        /// Ключевые сегменты, которые были пересечены.
        /// </summary>
        public SurvivalStatKeySegment[] KeySegments { get; }
    }
}
