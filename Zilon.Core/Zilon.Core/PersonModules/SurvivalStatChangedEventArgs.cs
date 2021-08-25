﻿using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Persons.Survival;

namespace Zilon.Core.PersonModules
{
    /// <summary>
    /// Аргументы события при изменения характеристики.
    /// </summary>
    public class SurvivalStatChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="stat">Характеристика, которая была изменена.</param>
        [ExcludeFromCodeCoverage]
        public SurvivalStatChangedEventArgs(SurvivalStat stat)
        {
            Stat = stat ?? throw new ArgumentNullException(nameof(stat));
        }

        /// <summary>
        /// Характеристика, которая изменялась.
        /// </summary>
        public SurvivalStat Stat { get; }
    }
}