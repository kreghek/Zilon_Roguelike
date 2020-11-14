using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Zilon.Core.Persons;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Структура для передачи действий, применённых за один ход.
    /// </summary>
    public sealed class UsedTacticalActs
    {
        [ExcludeFromCodeCoverage]
        public UsedTacticalActs([NotNull] [ItemNotNull] IEnumerable<ITacticalAct> primary) :
            this(primary, new ITacticalAct[0])
        {
        }

        [ExcludeFromCodeCoverage]
        public UsedTacticalActs([NotNull] [ItemNotNull] IEnumerable<ITacticalAct> primary,
            [NotNull] [ItemNotNull] IEnumerable<ITacticalAct> secondary)
        {
            Primary = primary ?? throw new ArgumentNullException(nameof(primary));
            Secondary = secondary ?? throw new ArgumentNullException(nameof(secondary));
        }

        public IEnumerable<ITacticalAct> Primary { get; }
        public IEnumerable<ITacticalAct> Secondary { get; }
    }
}