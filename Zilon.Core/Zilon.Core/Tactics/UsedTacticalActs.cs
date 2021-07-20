using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Persons;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Структура для передачи действий, применённых за один ход.
    /// </summary>
    public sealed class UsedTacticalActs
    {
        [ExcludeFromCodeCoverage]
        public UsedTacticalActs(IEnumerable<ICombatAct> primary) :
            this(primary, new ICombatAct[0])
        {
        }

        [ExcludeFromCodeCoverage]
        public UsedTacticalActs(IEnumerable<ICombatAct> primary,
            IEnumerable<ICombatAct> secondary)
        {
            Primary = primary ?? throw new ArgumentNullException(nameof(primary));
            Secondary = secondary ?? throw new ArgumentNullException(nameof(secondary));
        }

        public IEnumerable<ICombatAct> Primary { get; }
        public IEnumerable<ICombatAct> Secondary { get; }
    }
}