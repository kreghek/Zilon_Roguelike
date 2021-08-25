﻿using System;
using System.Diagnostics.CodeAnalysis;

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
            PrefferedDefenceItem =
                prefferedDefenceItem ?? throw new ArgumentNullException(nameof(prefferedDefenceItem));
            SuccessToHitRoll = successToHitRoll;
            FactToHitRoll = factToHitRoll;
        }

        /// <summary>
        /// Фактический бросок, который был выполнен для пробития обороны.
        /// </summary>
        public int FactToHitRoll { get; }

        /// <summary>
        /// Оборона, которая была использована.
        /// </summary>
        public PersonDefenceItem PrefferedDefenceItem { get; }

        /// <summary>
        /// Бросок, который был необходим для того, чтобы пробить оборону.
        /// </summary>
        public int SuccessToHitRoll { get; }
    }
}