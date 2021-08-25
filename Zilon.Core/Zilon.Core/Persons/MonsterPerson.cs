﻿using System;

using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Персонаж для монстров в секторе.
    /// </summary>
    public class MonsterPerson : PersonBase
    {
        public MonsterPerson(IMonsterScheme scheme) : base(Fractions.MonsterFraction)
        {
            Scheme = scheme ?? throw new ArgumentNullException(nameof(scheme));
        }

        /// <inheritdoc />
        public override int Id { get; set; }

        /// <inheritdoc />
        public override PhysicalSizePattern PhysicalSize => PhysicalSizePattern.Size1;

        /// <inheritdoc />
        public IMonsterScheme Scheme { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Scheme?.Name?.En}";
        }
    }
}