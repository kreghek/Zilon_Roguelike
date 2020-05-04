using System;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.PersonModules;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Персонаж для монстров в секторе.
    /// </summary>
    public class MonsterPerson : PersonBase
    {
        /// <inheritdoc/>
        public override int Id { get; set; }

        /// <inheritdoc/>
        public IMonsterScheme Scheme { get; }

        /// <inheritdoc/>
        public override PhysicalSize PhysicalSize { get => PhysicalSize.Size1; }

        public MonsterPerson([NotNull] IMonsterScheme scheme) : base()
        {
            Scheme = scheme ?? throw new ArgumentNullException(nameof(scheme));
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Scheme?.Name?.En}";
        }
    }
}