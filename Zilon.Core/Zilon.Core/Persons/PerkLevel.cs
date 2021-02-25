using System;
using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core.Persons
{
    public sealed class PerkLevel
    {
        public PerkLevel(int primary, int sub)
        {
            if (primary <= 0)
            {
                throw new ArgumentException($"Value must be positive non-zero integer number. Currently is {primary}.",
                    nameof(primary));
            }

            if (sub <= 0)
            {
                throw new ArgumentException($"Value must be positive non-zero integer number. Currently is {sub}.",
                    nameof(sub));
            }

            Primary = primary;
            Sub = sub;
        }

        /// <summary>
        /// Основной уровень перка. Иначе - индекс схемы уровня.
        /// </summary>
        public int Primary { get; }

        /// <summary>
        /// Подуровень. Или уровень внутри схемы уровня перка.
        /// </summary>
        public int Sub { get; }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return $"lvl:{Primary} sub:{Sub}";
        }
    }
}