using System;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

using Zilon.Core.Persons;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Аргументы события, которое выстреливает, когда актёр отрабатывает оборону.
    /// </summary>
    public sealed class DefenceEventArgs : EventArgs
    {
        [ExcludeFromCodeCoverage]
        public DefenceEventArgs([NotNull] PersonDefenceItem prefferedDefenceItem,
            int successToHitRoll,
            int factToHitRoll)
        {
            PrefferedDefenceItem = prefferedDefenceItem ?? throw new ArgumentNullException(nameof(prefferedDefenceItem));
            SuccessToHitRoll = successToHitRoll;
            FactToHitRoll = factToHitRoll;
        }

        /// <summary>
        /// Оборона, которая была использована.
        /// </summary>
        [PublicAPI]
        public PersonDefenceItem PrefferedDefenceItem { get; }

        /// <summary>
        /// Бросок, который был необходим для того, чтобы пробить оборону.
        /// </summary>
        [PublicAPI]
        public int SuccessToHitRoll { get; }

        /// <summary>
        /// Фактический бросок, который был выполнен для пробития обороны.
        /// </summary>
        [PublicAPI]
        public int FactToHitRoll { get; }
    }
}