using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core.Persons
{
    [ExcludeFromCodeCoverage]
    public sealed class PerkLevel
    {
        public PerkLevel(int primary, int sub)
        {
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

        public override string ToString()
        {
            return $"lvl:{Primary} sub:{Sub}";
        }
    }
}